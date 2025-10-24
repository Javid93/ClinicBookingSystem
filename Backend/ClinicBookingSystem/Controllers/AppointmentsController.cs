using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicBookingSystem.Data;
using ClinicBookingSystem.Models;
using ClinicBookingSystem.DTOs;
using System.Globalization;

namespace ClinicBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ClinicContext _context;
        public AppointmentsController(ClinicContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Gets all appointments.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointments()
        {
            var oslo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .ToListAsync();

            var dtoList = appointments.Select(a => new AppointmentDTO
            {
                Id = a.Id,
                AppointmentDateTime = a.AppointmentDateTime.ToString("yyyy-MM-ddTHH:mm"),
                DurationMinutes = a.DurationMinutes,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                ClinicId = a.ClinicId,
                Status = a.Status
            }).ToList();

            return Ok(dtoList);
        }


        /// <summary>
        /// Creates a new appointment. Validates time, clash, and clinic hours.
        /// </summary>
        /// <remarks>
        /// - Time must be in "yyyy-MM-ddTHH:mm" format (Oslo time)
        /// - Must be at least 30 minutes in advance
        /// - Appointments must be booked on weekdays (Monday–Friday) between 09:00 and 16:00 (Oslo time)
        /// - Cannot clash with existing appointments for the same patient
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<AppointmentDTO>> CreateAppointment(AppointmentDTO dto)
        {


            // Convert parsed date to UTC (assuming it's Oslo time)
            var oslo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            if (!DateTime.TryParseExact(dto.AppointmentDateTime, "yyyy-MM-ddTHH:mm", null, DateTimeStyles.None, out var parsedDateTime))
                return BadRequest("Invalid datetime format. Use yyyy-MM-ddTHH:mm.");
            var appointmentUtc = TimeZoneInfo.ConvertTimeToUtc(parsedDateTime, oslo);

            if (appointmentUtc < DateTime.UtcNow)
                return BadRequest("You cannot book an appointment in the past.");

            // Enforce 30-minute rule
            if (appointmentUtc < DateTime.UtcNow.AddMinutes(30))
                return BadRequest("Appointments must be booked at least 30 minutes in advance.");


            //Validation working hours
            if (parsedDateTime.TimeOfDay.Add(TimeSpan.FromMinutes(dto.DurationMinutes)) > TimeSpan.FromHours(16))
            {
                return BadRequest("Appointment duration exceeds clinic closing time (16:00) Please select a shorter duration.");
            }


            // Check if the patient already has an appointments at this exact time.
            var hasPatientConflict = await _context.Appointments.AnyAsync(a =>
            a.PatientId == dto.PatientId &&
            a.AppointmentDateTime == parsedDateTime);

            if (hasPatientConflict)
                return Conflict("This patient already has an appointment at that time.");

            //Check if the doctor is already booked at this overlapping time.
            var hasDoctorConflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == dto.DoctorId &&
                parsedDateTime < a.AppointmentDateTime.AddMinutes(a.DurationMinutes) &&
                parsedDateTime.AddMinutes(dto.DurationMinutes) > a.AppointmentDateTime
            );

            if (hasDoctorConflict)
                return BadRequest("This time slot is already booked  with the selected doctor.");

            var appointment = new Appointment
            {
                AppointmentDateTime = parsedDateTime,
                DurationMinutes = dto.DurationMinutes,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                ClinicId = dto.ClinicId,
                Status = dto.Status ?? "Pending"
            };


            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var responseDto = new AppointmentDTO
            {
                Id = appointment.Id,
                AppointmentDateTime = appointment.AppointmentDateTime.ToString("yyyy-MM-ddTHH:mm"),
                DurationMinutes = appointment.DurationMinutes,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                ClinicId = appointment.ClinicId,
                Status = appointment.Status
            };

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, responseDto);

        }

        private bool IsValidAppointmentTime(DateTime appointmentDateTime)
        {
            // Only Monday-Friday between 09:00 and 16:00 (Oslo time) is allowed
            var dayOfWeek = appointmentDateTime.DayOfWeek;
            var time = appointmentDateTime.TimeOfDay;

            bool isWeekday = dayOfWeek >= DayOfWeek.Monday && dayOfWeek <= DayOfWeek.Friday;
            bool IsWithinWorkingHours = time >= TimeSpan.FromHours(9) && time < TimeSpan.FromHours(16);

            return isWeekday && IsWithinWorkingHours;
        }




        /// <summary>
        /// Gets a specific appointment by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDTO>> GetAppointment(int id)
        {
            var oslo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            var dto = new AppointmentDTO
            {
                Id = appointment.Id,
                AppointmentDateTime = appointment.AppointmentDateTime.ToString("yyyy-MM-ddTHH:mm"),

                DurationMinutes = appointment.DurationMinutes,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                ClinicId = appointment.ClinicId,
                Status = appointment.Status

            };

            return Ok(dto);


        }




        /// <summary>
        /// Updates an existing appointment. Validates time, clash, and clinic hours.
        /// </summary>
        /// <remarks>
        /// - Time must be in "yyyy-MM-ddTHH:mm" format (Oslo time)
        /// - Must be at least 30 minutes in advance
        /// - Appointments must be booked on weekdays (Monday–Friday) between 09:00 and 16:00 (Oslo time)
        /// - Cannot clash with existing appointments for the same patient
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, AppointmentDTO dto)
        {

            if (id != dto.Id)
                return BadRequest("Mismatch between route ID and body ID.");


            var oslo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            if (!DateTime.TryParseExact(dto.AppointmentDateTime, "yyyy-MM-ddTHH:mm", null, DateTimeStyles.None, out var parsedDateTime))
                return BadRequest("Invalid datetime format. Use yyyy-MM-ddTHH:mm.");
            var appointmentUtc = TimeZoneInfo.ConvertTimeToUtc(parsedDateTime, oslo);


            if (appointmentUtc < DateTime.UtcNow)
                return BadRequest("You cannot book an appointment in the past");



            //Allow booking only at least 30 minutes from now
            if (appointmentUtc < DateTime.UtcNow.AddMinutes(30))
                return BadRequest("Appointments must be booked at least 30 minutes in advance.");

            //Validation working hours
            if (parsedDateTime.TimeOfDay.Add(TimeSpan.FromMinutes(dto.DurationMinutes)) > TimeSpan.FromHours(16))
            {
                return BadRequest("Appointment duration exceeds clinic closing time (16:00) Please select a shorter duration.");
            }

            //Now check if appointment exists
            var existing = await _context.Appointments.FindAsync(id);
            if (existing == null)
                return NotFound();


            // Check if the patient already has an appointments at this exact time.
            var hasPatientConflict = await _context.Appointments.AnyAsync(a =>
                a.Id != id &&
                a.PatientId == dto.PatientId &&
                a.AppointmentDateTime == parsedDateTime);

            if (hasPatientConflict)
                return Conflict("This patient  already has an appointment at that time.");



            //Check if the doctor is already booked at this overlapping time.
            var hasDoctorConflict = await _context.Appointments.AnyAsync(a =>
                a.Id != id &&
                a.DoctorId == dto.DoctorId &&
                parsedDateTime < a.AppointmentDateTime.AddMinutes(a.DurationMinutes) &&
                parsedDateTime.AddMinutes(dto.DurationMinutes) > a.AppointmentDateTime
            );

            if (hasDoctorConflict)
                return BadRequest("This time slot overlaps with another for the selected doctor.");






            //Update fields
            existing.AppointmentDateTime = parsedDateTime;
            existing.DurationMinutes = dto.DurationMinutes;
            existing.PatientId = dto.PatientId;
            existing.DoctorId = dto.DoctorId;
            existing.ClinicId = dto.ClinicId;
            existing.Status = dto.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }




        /// <summary>
        /// Deletes an appointment by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// Searches for appointments on a specific date.
        /// Example: search date=2025-05-14
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> SearchAppointmentsByDate([FromQuery] string date)
        {
            if (!DateTime.TryParse(date, out var targetDate))
                return BadRequest("Invalid date format. Use yyyy-MM-dd.");

            var oslo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");


            var appointments = await _context.Appointments
                .Where(a => a.AppointmentDateTime.Date == targetDate.Date)
                .Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    AppointmentDateTime = TimeZoneInfo.ConvertTimeFromUtc(a.AppointmentDateTime, oslo).ToString("yyyy-MM-ddTHH:mm"),
                    DurationMinutes = a.DurationMinutes,
                    PatientId = a.PatientId,
                    DoctorId = a.DoctorId,
                    ClinicId = a.ClinicId,
                    Status = a.Status

                })
                .ToListAsync();

            return Ok(appointments);
        }


    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicBookingSystem.Data;
using ClinicBookingSystem.Models;
using ClinicBookingSystem.DTOs;


namespace ClinicBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class DoctorsController : ControllerBase
    {
        private readonly ClinicContext _context;

        public DoctorsController(ClinicContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of all doctors with their clinic and speciality details.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Speciality)
                .ToListAsync();

            var dtoList = doctors.Select(d => new DoctorDTO
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                ClinicId = d.ClinicId,
                SpecialityId = d.SpecialityId
            });

            return Ok(dtoList);
        }



        /// <summary>
        /// Creates a new doctor entry in the system after validating uniqueness within the clinic.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DoctorDTO>> CreateDoctor(DoctorDTO dto)
        {
            var exists = await _context.Doctors.AnyAsync(d =>
                d.FirstName == dto.FirstName &&
                d.LastName == dto.LastName &&
                d.ClinicId == dto.ClinicId);

            if (exists)
                return Conflict("A doctor with the same name already exists in this clinic.");

            var doctor = new Doctor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                ClinicId = dto.ClinicId,
                SpecialityId = dto.SpecialityId

            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();


            dto.Id = doctor.Id;
            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, dto);
        }



        /// <summary>
        /// Returns a specific doctor by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Speciality)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound();

            var dto = new DoctorDTO
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                ClinicId = doctor.ClinicId,
                SpecialityId = doctor.SpecialityId
            };

            return Ok(dto);
        }


        /// <summary>
        /// Updates an existing doctor's details.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, DoctorDTO dto)
        {
            if (id != dto.Id)
                return BadRequest("Doctor ID mismatch.");
            var exists = await _context.Doctors.AnyAsync(d =>
                d.FirstName == dto.FirstName &&
                d.LastName == dto.LastName &&
                d.ClinicId == dto.ClinicId);

            if (exists)
                return Conflict("A doctor with the same name already exists in this clinic.");

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            doctor.FirstName = dto.FirstName;
            doctor.LastName = dto.LastName;
            doctor.ClinicId = dto.ClinicId;
            doctor.SpecialityId = dto.SpecialityId;

            await _context.SaveChangesAsync();
            return NoContent();
        }



        /// <summary>
        /// Deletes a doctor by ID. Fails if the doctor has appointments.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();


            bool hasAppointments = await _context.Appointments.AnyAsync(a => a.DoctorId == id);
            if (hasAppointments)
                return Conflict("Doctor has appointments and cannot be deleted.");

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// Searches for doctors by first or last name.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchDoctors([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Search term is required.");

            var results = await _context.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Speciality)
                .Where(d => d.FirstName.ToLower().Contains(name.ToLower()) || d.LastName.ToLower().Contains(name.ToLower()))
                .Select(d => new
                {
                    FullName = d.FirstName + " " + d.LastName,
                    ClinicName = d.Clinic.Name,
                    SpecialityName = d.Speciality.Name
                })
                .ToListAsync();

            if (!results.Any())
                return NotFound("No matching doctors found.");

            return Ok(results);
        }


        ///<summary>
        /// Returns available and booked 30-minute slots for a doctor on a specific date.
        /// </summary>
        /// <remarks>
        /// - Input format: yyyy-MM-dd
        /// - Returns available and booked slots separately
        /// </remarks>
        [HttpGet("{id}/availability")]
        public async Task<ActionResult> GetAvailability(int id, [FromQuery] string date)
        {
            if (!DateTime.TryParse(date, out var targetDate))
                return BadRequest("Invalid date format. Use yyyy-MM-dd,");

            //Only allow current or future dates
            if (targetDate.Date < DateTime.UtcNow.Date)
                return BadRequest("Cannot check availability for past dates.");

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound("Doctor not found.");

            var existingAppointments = await _context.Appointments
                .Where(a => a.DoctorId == id && a.AppointmentDateTime.Date == targetDate.Date)
                .Select(a => a.AppointmentDateTime.TimeOfDay)
                .ToListAsync();

            var workingStart = new TimeSpan(9, 0, 0);
            var workingEnd = new TimeSpan(16, 0, 0);
            var slotLength = new TimeSpan(0, 30, 0);

            var availableSlots = new List<string>();
            var bookedSlots = new List<string>();

            for (var time = workingStart; time < workingEnd; time += slotLength)
            {
                var formattedTime = time.ToString(@"hh\:mm");
                if (existingAppointments.Contains(time))
                    bookedSlots.Add(formattedTime);
                else
                    availableSlots.Add(formattedTime);
            }

            return Ok(new
            {
                doctorId = id,
                date = targetDate.ToString("yyyy-MM-dd"),
                availableSlots,
                bookedSlots
            });
        }



    }
}
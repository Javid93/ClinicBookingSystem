using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicBookingSystem.Data;
using ClinicBookingSystem.Models;
using ClinicBookingSystem.DTOs;

namespace ClinicBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ClinicContext _context;

        public PatientsController(ClinicContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all patients.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients()
        {
            var patients = await _context.Patients.ToListAsync();
            var dtoList = patients.Select(p => new PatientDTO
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                BirthDate = p.BirthDate.ToString("yyyy-MM-dd"),
                Gender = p.Gender,
                TaxNumber = p.TaxNumber,
                Religion = p.Religion
            }).ToList();

            return Ok(dtoList);
        }


        /// <summary>
        /// Creates a new patient. Validates uniqueness of email and birth date.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PatientDTO>> CreatePatient(PatientDTO dto)
        {
            var parsedBirthDate = DateTime.ParseExact(dto.BirthDate, "yyyy-MM-dd", null);

            var exists = await _context.Patients.AnyAsync(p =>
                p.Email == dto.Email &&
                p.BirthDate == parsedBirthDate);

            if (exists)
                return Conflict("A patient with the same email and birth date already exists.");

            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                BirthDate = parsedBirthDate,
                Gender = dto.Gender,
                TaxNumber = dto.TaxNumber,
                Religion = dto.Religion
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            dto.Id = patient.Id;

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, dto);
        }


        /// <summary>
        /// Gets patient by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDTO>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound();

            var dto = new PatientDTO
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                BirthDate = patient.BirthDate.ToString("yyyy-MM-dd"),
                Gender = patient.Gender,
                TaxNumber = patient.TaxNumber,
                Religion = patient.Religion
            };

            return Ok(dto);
        }


        /// <summary>
        /// Update an existing patient.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, PatientDTO dto)
        {
            if (id != dto.Id)
                return BadRequest("Patient ID mismatch.");

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            var parsedBirthDate = DateTime.ParseExact(dto.BirthDate, "yyyy-MM-dd", null);

            var exists = await _context.Patients.AnyAsync(p =>
                p.Email == dto.Email &&
                p.BirthDate == parsedBirthDate);

            if (exists)
                return Conflict("A patient with the same email and birth date already exists.");

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Email = dto.Email;
            patient.BirthDate = parsedBirthDate;
            patient.Gender = dto.Gender;
            patient.TaxNumber = dto.TaxNumber;
            patient.Religion = dto.Religion;

            await _context.SaveChangesAsync();


            return NoContent();
        }


        /// <summary>
        /// Deletes a patient by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            bool hasAppointments = await _context.Appointments.AnyAsync(a => a.PatientId == id);
            if (hasAppointments)
                return Conflict("Cannot delete patient with existing appointments.");

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
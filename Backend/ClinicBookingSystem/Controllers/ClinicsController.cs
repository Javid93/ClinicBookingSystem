using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicBookingSystem.Data;
using ClinicBookingSystem.Models;
using ClinicBookingSystem.DTOs;

namespace ClinicBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicsController : ControllerBase
    {
        private readonly ClinicContext _context;

        public ClinicsController(ClinicContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all Clinics.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetClinics()
        {
            var clinics = await _context.Clinics.ToListAsync();
            var dtoList = clinics.Select(c => new ClinicDTO
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address!
            }).ToList();

            return Ok(dtoList);
        }


        /// <summary>
        /// Creates a new clinic.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClinicDTO>> CreateClinic(ClinicDTO dto)
        {

            //Optional: check for duplicate name
            bool exists = await _context.Clinics.AnyAsync(c => c.Name == dto.Name);
            if (exists)
                return Conflict("A clinic with the same name already exists.");

            var clinic = new Clinic
            {
                Name = dto.Name,
                Address = dto.Address
            };

            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();

            var resultDto = new ClinicDTO
            {
                Id = clinic.Id,
                Name = clinic.Name,
                Address = clinic.Address!
            };

            return CreatedAtAction(nameof(GetClinic), new { id = clinic.Id }, resultDto);
        }


        /// <summary>
        /// Gets Clinic by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDTO>> GetClinic(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();


            var dto = new ClinicDTO
            {
                Id = clinic.Id,
                Name = clinic.Name,
                Address = clinic.Address!
            };

            return Ok(dto);
        }


        /// <summary>
        /// Updates an existing clinic.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClinic(int id, ClinicDTO dto)
        {
            if (id != dto.Id)
                return BadRequest("Clinic ID mismatch.");

            //Optional: check for duplicate name
            bool exists = await _context.Clinics.AnyAsync(c => c.Name == dto.Name);
            if (exists)
                return Conflict("A clinic with the same name already exists.");

            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();

            clinic.Name = dto.Name;
            clinic.Address = dto.Address;

            await _context.SaveChangesAsync();
            return NoContent();

        }


        /// <summary>
        /// Deletes a clinic by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();

            //Optional: Validate no dependent doctor or appointments exist
            bool hasDependencies = await _context.Doctors.AnyAsync(d => d.ClinicId == id) ||
                                   await _context.Appointments.AnyAsync(a => a.ClinicId == id);

            if (hasDependencies)
                return Conflict("Clinic has doctors or appointments and cannot be deleted.");

            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();

            return NoContent();
        }




    }
}
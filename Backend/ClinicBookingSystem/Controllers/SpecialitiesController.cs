using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicBookingSystem.Data;
using ClinicBookingSystem.Models;
using ClinicBookingSystem.DTOs;

namespace ClinicBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialitiesController : ControllerBase
    {
        private readonly ClinicContext _context;

        public SpecialitiesController(ClinicContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all specialities.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecialityDTO>>> GetSpecialities()
        {
            var specialities = await _context.Specialities.ToListAsync();

            var dtoList = specialities.Select(s => new SpecialityDTO
            {
                Id = s.Id,
                Name = s.Name
            });

            return Ok(dtoList);

        }


        /// <summary>
        /// Creates a new speciality. Validates uniqueness.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SpecialityDTO>> CreateSpeciality(SpecialityDTO dto)
        {
            bool exists = await _context.Specialities.AnyAsync(s => s.Name == dto.Name);
            if (exists)
                return Conflict("A speciality with same name already exists.");


            var speciality = new Speciality
            {
                Name = dto.Name
            };

            _context.Specialities.Add(speciality);
            await _context.SaveChangesAsync();

            var resultDto = new SpecialityDTO
            {
                Id = speciality.Id,
                Name = speciality.Name
            };

            return CreatedAtAction(nameof(GetSpeciality), new { id = dto.Id }, resultDto);
        }


        /// <summary>
        /// Gets a specific speciality by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialityDTO>> GetSpeciality(int id)
        {
            var speciality = await _context.Specialities.FindAsync(id);

            if (speciality == null)
                return NotFound();

            return new SpecialityDTO
            {
                Id = speciality.Id,
                Name = speciality.Name
            };
        }


        /// <summary>
        /// Updates an existing speciality.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpeciality(int id, SpecialityDTO dto)
        {
            if (id != dto.Id)
                return BadRequest("Speciality ID mismatch.");

            bool exists = await _context.Specialities.AnyAsync(s => s.Name == dto.Name);
            if (exists)
                return Conflict("A speciality with same name already exists.");

            var speciality = await _context.Specialities.FindAsync(id);
            if (speciality == null)
                return NotFound();

            speciality.Name = dto.Name;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        /// <summary>
        /// Deletes a speciality by ID. Fails if in use by doctors.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpeciality(int id)
        {
            var speciality = await _context.Specialities.FindAsync(id);
            if (speciality == null)
                return NotFound();


            bool hasDoctors = await _context.Doctors.AnyAsync(d => d.SpecialityId == id);
            if (hasDoctors)
                return Conflict("Speciality is assigned to doctors and cannot be deleted.");

            _context.Specialities.Remove(speciality);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
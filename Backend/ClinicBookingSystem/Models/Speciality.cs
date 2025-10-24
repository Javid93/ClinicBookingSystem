using System.ComponentModel.DataAnnotations;

namespace ClinicBookingSystem.Models
{
    public class Speciality
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        // Navigation property: One speciality can be shared by many doctors
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
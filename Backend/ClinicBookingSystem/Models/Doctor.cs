using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicBookingSystem.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = default!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = default!;

        [NotMapped]
        public string Fullname => $"{FirstName} {LastName}";

        //Foreign key to Clinic
        [Required]
        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; } = default!;

        // Foreign key to Speciality
        [Required]
        public int SpecialityId { get; set; }
        public Speciality Speciality { get; set; } = default!;

        //Navigation properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    }
}
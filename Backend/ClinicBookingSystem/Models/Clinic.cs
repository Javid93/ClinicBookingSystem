using System.ComponentModel.DataAnnotations;

namespace ClinicBookingSystem.Models
{
    public class Clinic
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = default!;


        //Navigation properties
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
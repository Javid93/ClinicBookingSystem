using System.ComponentModel.DataAnnotations;

namespace ClinicBookingSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Range(1, 180, ErrorMessage = "Duration must be between 1 and 180 minutes.")]
        public int DurationMinutes { get; set; }

        //Foreign Key to Patient
        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = default!;

        //Foreign Key to Doctor
        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = default!;

        //Foreign key to Clinic
        [Required]
        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; } = default!;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
    }
}

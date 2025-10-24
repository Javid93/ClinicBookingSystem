namespace ClinicBookingSystem.DTOs
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public string AppointmentDateTime { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
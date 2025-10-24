namespace ClinicBookingSystem.DTOs
{
    public class DoctorDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int ClinicId { get; set; }
        public int SpecialityId { get; set; }
    }
}
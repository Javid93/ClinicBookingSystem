using Swashbuckle.AspNetCore.Annotations;

namespace ClinicBookingSystem.DTOs
{
    public class PatientDTO

    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Birth date in format yyyy-MM-dd")]
        [System.ComponentModel.DataAnnotations.RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Format must be yyyy-MM-dd")]
        public string BirthDate { get; set; } = "1990-05-14";

        public string? Gender { get; set; }
        public string? TaxNumber { get; set; }
        public string? Religion { get; set; }
    }
}
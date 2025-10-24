using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ClinicBookingSystem.Filters
{
    public class AppointmentDtoExampleFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.Name == "AppointmentDTO")
            {
                schema.Example = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(0),
                    ["appointmentDateTime"] = new OpenApiString(
                    TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time")
                    ).ToString("yyyy-MM-ddTHH:mm")
                    ),

                    ["durationMinutes"] = new OpenApiInteger(30),
                    ["patientId"] = new OpenApiInteger(0),
                    ["doctorId"] = new OpenApiInteger(0),
                    ["clinicId"] = new OpenApiInteger(0),
                    ["status"] = new OpenApiString("Pending")
                };
            }
        }
    }
}

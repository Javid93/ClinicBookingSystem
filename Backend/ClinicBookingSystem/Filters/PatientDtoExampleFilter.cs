using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ClinicBookingSystem.Filters
{
    public class PatientDtoExampleFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.Name == "PatientDTO")
            {
                schema.Example = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(0),
                    ["firstName"] = new OpenApiString("John"),
                    ["lastName"] = new OpenApiString("Doe"),
                    ["email"] = new OpenApiString("john.doe@example.com"),
                    ["birthDate"] = new OpenApiString("1990-05-14"),
                    ["gender"] = new OpenApiString("Male"),
                    ["taxNumber"] = new OpenApiString("123456789"),
                    ["religion"] = new OpenApiString("None")
                };
            }
        }
    }
}

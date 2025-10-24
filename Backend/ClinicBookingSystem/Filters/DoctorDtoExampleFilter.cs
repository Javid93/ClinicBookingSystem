using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ClinicBookingSystem.Filters
{
    public class DoctorDtoExampleFilter : ISchemaFilter
    {


        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.Name == "DoctorDTO")
            {
                schema.Example = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(0),
                    ["firstName"] = new OpenApiString("Ola"),
                    ["lastName"] = new OpenApiString("Nordmann"),
                    ["clinicId"] = new OpenApiInteger(1),
                    ["specialityId"] = new OpenApiInteger(2)
                };
            }
        }
    }
}
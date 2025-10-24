using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ClinicBookingSystem.Filters
{
    public class SpecialityDtoExampleFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.Name == "SpecialityDTO")
            {
                schema.Example = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(0),
                    ["name"] = new OpenApiString("Cardiology")
                };
            }
        }
    }
}
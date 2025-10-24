using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ClinicBookingSystem.Filters
{
    public class ClinicDtoExampleFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.Name == "ClinicDTO")
            {
                schema.Example = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(0),
                    ["name"] = new OpenApiString("Oslo Clinic"),
                    ["address"] = new OpenApiString("Karl Johans gate 1, 0154 Oslo")
                };
            }
        }
    }
}
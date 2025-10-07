using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using UAE_Pass_Poc.CustomAttributes;

namespace UAE_Pass_Poc.Filters
{
    public class SwaggerIgnoreFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context?.Type == null)
                return;

            var ignoredProperties = context.Type.GetProperties()
                .Where(prop => prop.GetCustomAttribute<SwaggerIgnoreAttribute>() != null);

            
            foreach (var prop in ignoredProperties)
            {
                var jsonPropertyName = char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);
                schema.Properties.Remove(jsonPropertyName);
            }
        }
    }
}

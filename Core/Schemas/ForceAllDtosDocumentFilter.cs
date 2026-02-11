using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EliseuBatista99.Core
{
    public class ForceAllDtosDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var dtoBaseType = typeof(Dto);

            var allDtoTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch
                    {
                        return Array.Empty<Type>();
                    }
                })
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    dtoBaseType.IsAssignableFrom(t)
                );

            foreach (var dtoType in allDtoTypes)
            {
                context.SchemaGenerator.GenerateSchema(
                    dtoType,
                    context.SchemaRepository
                );
            }
        }


    }
}

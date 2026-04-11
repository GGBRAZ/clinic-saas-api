using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ClinicSaaS.Api.Swagger;

public class AddClinicHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var controllerName = context.ApiDescription.ActionDescriptor.RouteValues["controller"];

        if (string.Equals(controllerName, "Clinics", StringComparison.OrdinalIgnoreCase))
            return;

        operation.Parameters ??= new List<IOpenApiParameter>();

        var alreadyExists = operation.Parameters.Any(p =>
            string.Equals(p.Name, "X-Clinic-Id", StringComparison.OrdinalIgnoreCase) &&
            p.In == ParameterLocation.Header);

        if (alreadyExists)
            return;

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Clinic-Id",
            In = ParameterLocation.Header,
            Required = true,
            Description = "Clinic identifier header",
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Format = "uuid"
            }
        });
    }
}
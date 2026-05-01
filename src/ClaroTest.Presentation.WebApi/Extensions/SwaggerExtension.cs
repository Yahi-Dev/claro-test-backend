using Microsoft.OpenApi.Models;

namespace ClaroTest.Presentation.WebApi.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwaggerExtension(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "ClaroTest API",
                Description = "API proxy sobre FakeRestAPI para la prueba técnica de Claro. " +
                              "Implementa Clean Architecture (Onion) con CQRS + MediatR.",
                Contact = new OpenApiContact { Name = "ClaroTest" }
            });

            // Identificadores de esquema legibles para tipos genéricos
            // (Response<IReadOnlyList<AuthorViewModel>> => ResponseOfIReadOnlyListOfAuthorViewModel).
            options.CustomSchemaIds(BuildSchemaId);
        });

        return services;
    }

    private static string BuildSchemaId(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        var baseName = type.Name.Substring(0, type.Name.IndexOf('`'));
        var argNames = type.GetGenericArguments().Select(BuildSchemaId);
        return $"{baseName}Of{string.Join("And", argNames)}";
    }

    public static IApplicationBuilder UseSwaggerExtension(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ClaroTest API v1");
            options.RoutePrefix = "swagger";
            options.DocumentTitle = "ClaroTest API";
        });

        return app;
    }
}

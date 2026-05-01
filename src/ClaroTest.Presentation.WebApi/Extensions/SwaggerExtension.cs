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

            // Generamos identificadores de esquema legibles para tipos genéricos
            // (ej: Response<IReadOnlyList<AuthorViewModel>> => Response_IReadOnlyList_AuthorViewModel).
            // Usar type.FullName produce nombres con backticks, comas y signos = que Swagger UI
            // no puede resolver como $ref, lo que causa el error "Unable to render this definition".
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
        // Inyectamos no-cache en la ruta de Swagger para evitar que un bundle antiguo
        // del navegador o un swagger.json cacheado provoque "Unable to render this definition".
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                    context.Response.Headers["Pragma"] = "no-cache";
                    context.Response.Headers["Expires"] = "0";
                    return Task.CompletedTask;
                });
            }
            await next();
        });

        app.UseSwagger(options =>
        {
            // Forzamos la serialización al formato OpenAPI 3.0, el más universalmente compatible
            // con todas las builds de Swagger UI (incluyendo cualquier versión cacheada).
            options.SerializeAsV2 = false;
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ClaroTest API v1");
            options.RoutePrefix = "swagger";
            options.DocumentTitle = "ClaroTest API";
        });

        return app;
    }
}

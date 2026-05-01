using ClaroTest.Presentation.WebApi.Middlewares;

namespace ClaroTest.Presentation.WebApi.Extensions;

public static class MiddlewareExtension
{
    public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ErrorHandleMiddleware>();
}

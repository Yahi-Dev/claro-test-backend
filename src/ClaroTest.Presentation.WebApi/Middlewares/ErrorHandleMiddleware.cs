using System.Net;
using System.Text.Json;
using ClaroTest.Core.Application.Wrappers;

namespace ClaroTest.Presentation.WebApi.Middlewares;

public class ErrorHandleMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandleMiddleware> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ErrorHandleMiddleware(RequestDelegate next, ILogger<ErrorHandleMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new Response<object>
        {
            Succeeded = false,
            Message = exception.Message
        };

        switch (exception)
        {
            case ClaroTest.Core.Application.Wrappers.ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Errors = validationEx.Errors;
                _logger.LogWarning("Validación fallida: {Errors}",
                    string.Join("; ", validationEx.Errors ?? new List<string>()));
                break;

            case NotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                _logger.LogInformation("Recurso no encontrado: {Message}", notFoundEx.Message);
                break;

            case ApiException apiEx:
                context.Response.StatusCode = apiEx.StatusCode;
                response.Errors = apiEx.Errors;
                _logger.LogError(apiEx, "Excepción de API manejada con status {StatusCode}", apiEx.StatusCode);
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Ocurrió un error inesperado.";
                _logger.LogError(exception, "Excepción no manejada");
                break;
        }

        var payload = JsonSerializer.Serialize(response, JsonOptions);
        await context.Response.WriteAsync(payload);
    }
}

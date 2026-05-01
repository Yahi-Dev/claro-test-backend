using System.Globalization;

namespace ClaroTest.Core.Application.Wrappers;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public List<string>? Errors { get; }

    public ApiException() : base() { }

    public ApiException(string message, int statusCode = 500, List<string>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }

    public ApiException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
        StatusCode = 500;
    }
}

public class ValidationException : ApiException
{
    public ValidationException(List<string> errors)
        : base("Ocurrieron uno o más errores de validación.", 400, errors) { }
}

public class NotFoundException : ApiException
{
    public NotFoundException(string name, object key)
        : base($"La entidad \"{name}\" ({key}) no fue encontrada.", 404) { }
}

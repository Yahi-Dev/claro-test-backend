namespace ClaroTest.Core.Application.Wrappers;

public class Response<T>
{
    public Response() { }

    public Response(T data, string? message = null)
    {
        Succeeded = true;
        Message = message;
        Data = data;
    }

    public Response(string message, List<string>? errors = null)
    {
        Succeeded = false;
        Message = message;
        Errors = errors;
    }

    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public T? Data { get; set; }

    public static Response<T> Success(T data, string? message = null) => new(data, message);
    public static Response<T> Failure(string message, List<string>? errors = null) => new(message, errors);
}

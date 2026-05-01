using System.Net;
using System.Text.Json;

namespace ClaroTest.Tests.Common;

/// <summary>
/// Test double que captura las peticiones HTTP salientes y devuelve respuestas predefinidas.
/// Permite ejercitar los repositorios sobre HttpClient tipado sin tráfico de red real.
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
    public List<HttpRequestMessage> Requests { get; } = new();

    public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    public static MockHttpMessageHandler RespondJson<T>(T payload, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(_ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                System.Text.Encoding.UTF8,
                "application/json")
        });

    public static MockHttpMessageHandler RespondStatus(HttpStatusCode statusCode)
        => new(_ => new HttpResponseMessage(statusCode));

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Requests.Add(request);
        return Task.FromResult(_responder(request));
    }
}

using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Infrastructure.ExternalApi.Repositories;
using ClaroTest.Infrastructure.ExternalApi.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace ClaroTest.Infrastructure.ExternalApi.IOC;

public static class ServiceRegistration
{
    public static IServiceCollection AddExternalApiInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FakeRestApiSettings>(
            configuration.GetSection(FakeRestApiSettings.SectionName));

        var settings = configuration
            .GetSection(FakeRestApiSettings.SectionName)
            .Get<FakeRestApiSettings>() ?? new FakeRestApiSettings();

        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
        {
            throw new InvalidOperationException(
                $"El valor de configuración '{FakeRestApiSettings.SectionName}:BaseUrl' es obligatorio.");
        }

        services
            .AddHttpClient<IBookRepository, BookRepository>(client =>
            {
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddStandardResilienceHandler(ConfigureResilience);

        services
            .AddHttpClient<IAuthorRepository, AuthorRepository>(client =>
            {
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddStandardResilienceHandler(ConfigureResilience);

        return services;
    }

    private static void ConfigureResilience(HttpStandardResilienceOptions options)
    {
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromMilliseconds(500);
        options.Retry.BackoffType = DelayBackoffType.Exponential;
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(15);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    }
}

using System.Net;
using System.Net.Http.Json;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.Wrappers;
using ClaroTest.Core.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ClaroTest.Infrastructure.ExternalApi.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private const string ResourcePath = "api/v1/Authors";
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthorRepository> _logger;

    public AuthorRepository(HttpClient httpClient, ILogger<AuthorRepository> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Author>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(ResourcePath, cancellationToken);
        await EnsureSuccessAsync(response);

        var authors = await response.Content.ReadFromJsonAsync<List<Author>>(cancellationToken: cancellationToken);
        return authors ?? new List<Author>();
    }

    public async Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{ResourcePath}/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<Author>(cancellationToken: cancellationToken);
    }

    public async Task<List<Author>> GetByBookIdAsync(int idBook, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/v1/Authors/authors/books/{idBook}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new List<Author>();
        }

        await EnsureSuccessAsync(response);
        var authors = await response.Content.ReadFromJsonAsync<List<Author>>(cancellationToken: cancellationToken);
        return authors ?? new List<Author>();
    }

    public async Task<Author> AddAsync(Author entity, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(ResourcePath, entity, cancellationToken);
        await EnsureSuccessAsync(response);

        var created = await response.Content.ReadFromJsonAsync<Author>(cancellationToken: cancellationToken);
        return created ?? entity;
    }

    public async Task<Author> UpdateAsync(int id, Author entity, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"{ResourcePath}/{id}", entity, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException(nameof(Author), id);
        }

        await EnsureSuccessAsync(response);
        var updated = await response.Content.ReadFromJsonAsync<Author>(cancellationToken: cancellationToken);
        return updated ?? entity;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"{ResourcePath}/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException(nameof(Author), id);
        }

        await EnsureSuccessAsync(response);
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync();
        _logger.LogError(
            "FakeRestAPI devolvió {StatusCode} para {Method} {Uri}. Cuerpo: {Body}",
            (int)response.StatusCode,
            response.RequestMessage?.Method,
            response.RequestMessage?.RequestUri,
            body);

        throw new ApiException(
            $"La API externa devolvió {(int)response.StatusCode} ({response.ReasonPhrase}).",
            (int)response.StatusCode);
    }
}

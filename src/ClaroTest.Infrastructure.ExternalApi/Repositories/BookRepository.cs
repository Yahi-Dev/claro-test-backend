using System.Net;
using System.Net.Http.Json;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.Wrappers;
using ClaroTest.Core.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ClaroTest.Infrastructure.ExternalApi.Repositories;

public class BookRepository : IBookRepository
{
    private const string ResourcePath = "api/v1/Books";
    private readonly HttpClient _httpClient;
    private readonly ILogger<BookRepository> _logger;

    public BookRepository(HttpClient httpClient, ILogger<BookRepository> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(ResourcePath, cancellationToken);
        await EnsureSuccessAsync(response);

        var books = await response.Content.ReadFromJsonAsync<List<Book>>(cancellationToken: cancellationToken);
        return books ?? new List<Book>();
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{ResourcePath}/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<Book>(cancellationToken: cancellationToken);
    }

    public async Task<Book> AddAsync(Book entity, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(ResourcePath, entity, cancellationToken);
        await EnsureSuccessAsync(response);

        var created = await response.Content.ReadFromJsonAsync<Book>(cancellationToken: cancellationToken);
        return created ?? entity;
    }

    public async Task<Book> UpdateAsync(int id, Book entity, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"{ResourcePath}/{id}", entity, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException(nameof(Book), id);
        }

        await EnsureSuccessAsync(response);
        var updated = await response.Content.ReadFromJsonAsync<Book>(cancellationToken: cancellationToken);
        return updated ?? entity;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"{ResourcePath}/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException(nameof(Book), id);
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

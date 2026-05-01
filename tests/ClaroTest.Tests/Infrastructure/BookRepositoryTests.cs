using System.Net;
using ClaroTest.Core.Application.Wrappers;
using ClaroTest.Core.Domain.Entities;
using ClaroTest.Infrastructure.ExternalApi.Repositories;
using ClaroTest.Tests.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClaroTest.Tests.Infrastructure;

public class BookRepositoryTests
{
    private static BookRepository BuildSut(MockHttpMessageHandler handler)
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://fake.test/") };
        return new BookRepository(client, NullLogger<BookRepository>.Instance);
    }

    [Fact]
    public async Task GetAllAsync_DeserializesUpstreamPayload()
    {
        var handler = MockHttpMessageHandler.RespondJson(new[]
        {
            new Book { Id = 1, Title = "A" },
            new Book { Id = 2, Title = "B" }
        });
        var sut = BuildSut(handler);

        var books = await sut.GetAllAsync();

        books.Should().HaveCount(2);
        handler.Requests.Single().RequestUri!.AbsolutePath.Should().Be("/api/v1/Books");
    }

    [Fact]
    public async Task GetByIdAsync_WhenUpstreamReturns404_ReturnsNull()
    {
        var sut = BuildSut(MockHttpMessageHandler.RespondStatus(HttpStatusCode.NotFound));

        var book = await sut.GetByIdAsync(123);

        book.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenUpstreamReturns500_ThrowsApiException()
    {
        var sut = BuildSut(MockHttpMessageHandler.RespondStatus(HttpStatusCode.InternalServerError));

        var act = async () => await sut.DeleteAsync(1);

        var ex = await act.Should().ThrowAsync<ApiException>();
        ex.Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpstreamReturns404_ThrowsNotFoundException()
    {
        var sut = BuildSut(MockHttpMessageHandler.RespondStatus(HttpStatusCode.NotFound));

        var act = async () => await sut.UpdateAsync(1, new Book());

        await act.Should().ThrowAsync<NotFoundException>();
    }
}

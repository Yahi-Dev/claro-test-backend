using ClaroTest.Core.Application.Features.Authors.Queries.GetAllAuthors;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Domain.Entities;
using ClaroTest.Tests.Common;
using FluentAssertions;
using Moq;

namespace ClaroTest.Tests.Application.Authors;

public class GetAllAuthorsQueryHandlerTests
{
    [Fact]
    public async Task BookCount_IsAggregated_ByFullNameCaseInsensitive()
    {
        var repo = new Mock<IAuthorRepository>();
        repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Author>
            {
                new() { Id = 1, IdBook = 1, FirstName = "Ada",  LastName = "Lovelace" },
                new() { Id = 2, IdBook = 2, FirstName = "ADA",  LastName = "lovelace" }, // same identity, different casing
                new() { Id = 3, IdBook = 3, FirstName = "Alan", LastName = "Turing" }
            });
        var handler = new GetAllAuthorsQueryHandler(repo.Object, TestMapperFactory.Create());

        var result = await handler.Handle(new GetAllAuthorsQuery(), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().HaveCount(3);

        var ada = result.Data!.First(a => a.Id == 1);
        ada.BookCount.Should().Be(2, "Ada aparece en dos registros y la agregación debe ser case-insensitive");

        var alan = result.Data!.First(a => a.Id == 3);
        alan.BookCount.Should().Be(1);
    }
}

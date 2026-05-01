using ClaroTest.Core.Application.Features.Books.Commands.CreateBook;
using ClaroTest.Core.Application.Features.Books.Commands.DeleteBook;
using ClaroTest.Core.Application.Features.Books.Queries.GetAllBooks;
using ClaroTest.Core.Application.Features.Books.Queries.GetBookById;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.Wrappers;
using ClaroTest.Core.Domain.Entities;
using ClaroTest.Tests.Common;
using FluentAssertions;
using Moq;

namespace ClaroTest.Tests.Application.Books;

public class BookHandlerTests
{
    private readonly Mock<IBookRepository> _repository = new();

    [Fact]
    public async Task GetAllBooks_MapsRepositoryResult_ToViewModels()
    {
        _repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book>
            {
                new() { Id = 1, Title = "A", PageCount = 10 },
                new() { Id = 2, Title = "B", PageCount = 20 }
            });
        var handler = new GetAllBooksQueryHandler(_repository.Object, TestMapperFactory.Create());

        var result = await handler.Handle(new GetAllBooksQuery(), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].Title.Should().Be("A");
    }

    [Fact]
    public async Task GetBookById_WhenMissing_ThrowsNotFoundException()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);
        var handler = new GetBookByIdQueryHandler(_repository.Object, TestMapperFactory.Create());

        var act = async () => await handler.Handle(new GetBookByIdQuery(99), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateBook_DelegatesToRepository_AndReturnsViewModel()
    {
        _repository.Setup(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book b, CancellationToken _) => { b.Id = 42; return b; });
        var handler = new CreateBookCommandHandler(_repository.Object, TestMapperFactory.Create());

        var result = await handler.Handle(
            new CreateBookCommand { Title = "Brave New World", PageCount = 311, PublishDate = DateTime.UtcNow },
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Data!.Id.Should().Be(42);
        _repository.Verify(r => r.AddAsync(It.Is<Book>(b => b.Title == "Brave New World"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBook_ReturnsIdAndDelegates()
    {
        var handler = new DeleteBookCommandHandler(_repository.Object);

        var result = await handler.Handle(new DeleteBookCommand(7), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Data.Should().Be(7);
        _repository.Verify(r => r.DeleteAsync(7, It.IsAny<CancellationToken>()), Times.Once);
    }
}

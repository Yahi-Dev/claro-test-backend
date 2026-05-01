using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.Wrappers;
using MediatR;

namespace ClaroTest.Core.Application.Features.Books.Commands.DeleteBook;

public record DeleteBookCommand(int Id) : IRequest<Response<int>>;

public class DeleteBookCommandHandler
    : IRequestHandler<DeleteBookCommand, Response<int>>
{
    private readonly IBookRepository _repository;

    public DeleteBookCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Response<int>> Handle(
        DeleteBookCommand request,
        CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id, cancellationToken);
        return Response<int>.Success(request.Id, "Libro eliminado correctamente.");
    }
}

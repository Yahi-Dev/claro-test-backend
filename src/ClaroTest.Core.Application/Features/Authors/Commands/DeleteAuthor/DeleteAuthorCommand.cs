using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.Wrappers;
using MediatR;

namespace ClaroTest.Core.Application.Features.Authors.Commands.DeleteAuthor;

public record DeleteAuthorCommand(int Id) : IRequest<Response<int>>;

public class DeleteAuthorCommandHandler
    : IRequestHandler<DeleteAuthorCommand, Response<int>>
{
    private readonly IAuthorRepository _repository;

    public DeleteAuthorCommandHandler(IAuthorRepository repository)
    {
        _repository = repository;
    }

    public async Task<Response<int>> Handle(
        DeleteAuthorCommand request,
        CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id, cancellationToken);
        return Response<int>.Success(request.Id, "Autor eliminado correctamente.");
    }
}

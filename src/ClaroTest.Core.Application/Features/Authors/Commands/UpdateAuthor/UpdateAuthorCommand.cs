using AutoMapper;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.ViewModels.Authors;
using ClaroTest.Core.Application.Wrappers;
using ClaroTest.Core.Domain.Entities;
using MediatR;

namespace ClaroTest.Core.Application.Features.Authors.Commands.UpdateAuthor;

public class UpdateAuthorCommand : IRequest<Response<AuthorViewModel>>
{
    public int Id { get; set; }
    public int IdBook { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public class UpdateAuthorCommandHandler
    : IRequestHandler<UpdateAuthorCommand, Response<AuthorViewModel>>
{
    private readonly IAuthorRepository _repository;
    private readonly IMapper _mapper;

    public UpdateAuthorCommandHandler(IAuthorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<AuthorViewModel>> Handle(
        UpdateAuthorCommand request,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Author>(request);
        entity.Id = request.Id;
        var updated = await _repository.UpdateAsync(request.Id, entity, cancellationToken);
        var viewModel = _mapper.Map<AuthorViewModel>(updated);
        return Response<AuthorViewModel>.Success(viewModel, "Autor actualizado correctamente.");
    }
}

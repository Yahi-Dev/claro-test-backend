using AutoMapper;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.ViewModels.Authors;
using ClaroTest.Core.Application.Wrappers;
using ClaroTest.Core.Domain.Entities;
using MediatR;

namespace ClaroTest.Core.Application.Features.Authors.Commands.CreateAuthor;

public class CreateAuthorCommand : IRequest<Response<AuthorViewModel>>
{
    public int IdBook { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public class CreateAuthorCommandHandler
    : IRequestHandler<CreateAuthorCommand, Response<AuthorViewModel>>
{
    private readonly IAuthorRepository _repository;
    private readonly IMapper _mapper;

    public CreateAuthorCommandHandler(IAuthorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<AuthorViewModel>> Handle(
        CreateAuthorCommand request,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Author>(request);
        var created = await _repository.AddAsync(entity, cancellationToken);
        var viewModel = _mapper.Map<AuthorViewModel>(created);
        return Response<AuthorViewModel>.Success(viewModel, "Autor creado correctamente.");
    }
}

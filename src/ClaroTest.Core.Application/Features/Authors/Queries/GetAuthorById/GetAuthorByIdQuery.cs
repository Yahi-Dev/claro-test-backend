using AutoMapper;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.ViewModels.Authors;
using ClaroTest.Core.Application.Wrappers;
using MediatR;

namespace ClaroTest.Core.Application.Features.Authors.Queries.GetAuthorById;

public record GetAuthorByIdQuery(int Id) : IRequest<Response<AuthorViewModel>>;

public class GetAuthorByIdQueryHandler
    : IRequestHandler<GetAuthorByIdQuery, Response<AuthorViewModel>>
{
    private readonly IAuthorRepository _repository;
    private readonly IMapper _mapper;

    public GetAuthorByIdQueryHandler(IAuthorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<AuthorViewModel>> Handle(
        GetAuthorByIdQuery request,
        CancellationToken cancellationToken)
    {
        var author = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Author), request.Id);

        var viewModel = _mapper.Map<AuthorViewModel>(author);
        return Response<AuthorViewModel>.Success(viewModel);
    }
}

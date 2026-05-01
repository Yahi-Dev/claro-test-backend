using AutoMapper;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.ViewModels.Authors;
using ClaroTest.Core.Application.Wrappers;
using MediatR;

namespace ClaroTest.Core.Application.Features.Authors.Queries.GetAuthorsByBookId;

public record GetAuthorsByBookIdQuery(int IdBook) : IRequest<Response<IReadOnlyList<AuthorViewModel>>>;

public class GetAuthorsByBookIdQueryHandler
    : IRequestHandler<GetAuthorsByBookIdQuery, Response<IReadOnlyList<AuthorViewModel>>>
{
    private readonly IAuthorRepository _repository;
    private readonly IMapper _mapper;

    public GetAuthorsByBookIdQueryHandler(IAuthorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<IReadOnlyList<AuthorViewModel>>> Handle(
        GetAuthorsByBookIdQuery request,
        CancellationToken cancellationToken)
    {
        var authors = await _repository.GetByBookIdAsync(request.IdBook, cancellationToken);
        var viewModels = _mapper.Map<IReadOnlyList<AuthorViewModel>>(authors);
        return Response<IReadOnlyList<AuthorViewModel>>.Success(viewModels);
    }
}

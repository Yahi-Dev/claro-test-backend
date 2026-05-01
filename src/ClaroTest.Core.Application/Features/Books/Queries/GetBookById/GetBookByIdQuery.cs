using AutoMapper;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.ViewModels.Books;
using ClaroTest.Core.Application.Wrappers;
using MediatR;

namespace ClaroTest.Core.Application.Features.Books.Queries.GetBookById;

public record GetBookByIdQuery(int Id) : IRequest<Response<BookViewModel>>;

public class GetBookByIdQueryHandler
    : IRequestHandler<GetBookByIdQuery, Response<BookViewModel>>
{
    private readonly IBookRepository _repository;
    private readonly IMapper _mapper;

    public GetBookByIdQueryHandler(IBookRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<BookViewModel>> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Book), request.Id);

        var viewModel = _mapper.Map<BookViewModel>(book);
        return Response<BookViewModel>.Success(viewModel);
    }
}

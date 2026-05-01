using AutoMapper;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.ViewModels.Books;
using ClaroTest.Core.Application.Wrappers;
using MediatR;

namespace ClaroTest.Core.Application.Features.Books.Queries.GetAllBooks;

public record GetAllBooksQuery : IRequest<Response<IReadOnlyList<BookViewModel>>>;

public class GetAllBooksQueryHandler
    : IRequestHandler<GetAllBooksQuery, Response<IReadOnlyList<BookViewModel>>>
{
    private readonly IBookRepository _repository;
    private readonly IMapper _mapper;

    public GetAllBooksQueryHandler(IBookRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<IReadOnlyList<BookViewModel>>> Handle(
        GetAllBooksQuery request,
        CancellationToken cancellationToken)
    {
        var books = await _repository.GetAllAsync(cancellationToken);
        var viewModels = _mapper.Map<IReadOnlyList<BookViewModel>>(books);
        return Response<IReadOnlyList<BookViewModel>>.Success(viewModels);
    }
}

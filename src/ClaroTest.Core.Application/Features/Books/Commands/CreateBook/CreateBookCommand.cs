using AutoMapper;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.ViewModels.Books;
using ClaroTest.Core.Application.Wrappers;
using ClaroTest.Core.Domain.Entities;
using MediatR;

namespace ClaroTest.Core.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommand : IRequest<Response<BookViewModel>>
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public int PageCount { get; init; }
    public string? Excerpt { get; init; }
    public DateTime PublishDate { get; init; }
}

public class CreateBookCommandHandler
    : IRequestHandler<CreateBookCommand, Response<BookViewModel>>
{
    private readonly IBookRepository _repository;
    private readonly IMapper _mapper;

    public CreateBookCommandHandler(IBookRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<BookViewModel>> Handle(
        CreateBookCommand request,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Book>(request);
        var created = await _repository.AddAsync(entity, cancellationToken);
        var viewModel = _mapper.Map<BookViewModel>(created);
        return Response<BookViewModel>.Success(viewModel, "Libro creado correctamente.");
    }
}

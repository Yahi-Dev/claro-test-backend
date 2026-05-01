using AutoMapper;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.ViewModels.Books;
using ClaroTest.Core.Application.Wrappers;
using ClaroTest.Core.Domain.Entities;
using MediatR;

namespace ClaroTest.Core.Application.Features.Books.Commands.UpdateBook;

public class UpdateBookCommand : IRequest<Response<BookViewModel>>
{
    public int Id { get; set; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public int PageCount { get; init; }
    public string? Excerpt { get; init; }
    public DateTime PublishDate { get; init; }
}

public class UpdateBookCommandHandler
    : IRequestHandler<UpdateBookCommand, Response<BookViewModel>>
{
    private readonly IBookRepository _repository;
    private readonly IMapper _mapper;

    public UpdateBookCommandHandler(IBookRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<BookViewModel>> Handle(
        UpdateBookCommand request,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Book>(request);
        entity.Id = request.Id;
        var updated = await _repository.UpdateAsync(request.Id, entity, cancellationToken);
        var viewModel = _mapper.Map<BookViewModel>(updated);
        return Response<BookViewModel>.Success(viewModel, "Libro actualizado correctamente.");
    }
}

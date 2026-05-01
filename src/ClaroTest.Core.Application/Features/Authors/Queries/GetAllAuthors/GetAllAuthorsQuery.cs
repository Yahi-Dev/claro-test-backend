using AutoMapper;
using ClaroTest.Core.Application.Interfaces.Repositories;
using ClaroTest.Core.Application.ViewModels.Authors;
using ClaroTest.Core.Application.Wrappers;
using MediatR;

namespace ClaroTest.Core.Application.Features.Authors.Queries.GetAllAuthors;

public record GetAllAuthorsQuery : IRequest<Response<IReadOnlyList<AuthorViewModel>>>;

public class GetAllAuthorsQueryHandler
    : IRequestHandler<GetAllAuthorsQuery, Response<IReadOnlyList<AuthorViewModel>>>
{
    private readonly IAuthorRepository _repository;
    private readonly IMapper _mapper;

    public GetAllAuthorsQueryHandler(IAuthorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<IReadOnlyList<AuthorViewModel>>> Handle(
        GetAllAuthorsQuery request,
        CancellationToken cancellationToken)
    {
        var authors = await _repository.GetAllAsync(cancellationToken);

        // Agrupamos por nombre completo para obtener el número de libros publicados por cada autor.
        var counts = authors
            .GroupBy(a => BuildKey(a.FirstName, a.LastName))
            .ToDictionary(g => g.Key, g => g.Count());

        var viewModels = authors.Select(a =>
        {
            var vm = _mapper.Map<AuthorViewModel>(a);
            vm.BookCount = counts.GetValueOrDefault(BuildKey(a.FirstName, a.LastName), 0);
            return vm;
        }).ToList();

        return Response<IReadOnlyList<AuthorViewModel>>.Success(viewModels);
    }

    private static string BuildKey(string? first, string? last)
        => $"{first?.Trim().ToLowerInvariant()}|{last?.Trim().ToLowerInvariant()}";
}

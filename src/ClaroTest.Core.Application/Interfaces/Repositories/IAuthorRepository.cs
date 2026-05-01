using ClaroTest.Core.Domain.Entities;

namespace ClaroTest.Core.Application.Interfaces.Repositories;

public interface IAuthorRepository
{
    Task<List<Author>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Author>> GetByBookIdAsync(int idBook, CancellationToken cancellationToken = default);
    Task<Author> AddAsync(Author entity, CancellationToken cancellationToken = default);
    Task<Author> UpdateAsync(int id, Author entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

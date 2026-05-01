using ClaroTest.Core.Domain.Entities;

namespace ClaroTest.Core.Application.Interfaces.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Book> AddAsync(Book entity, CancellationToken cancellationToken = default);
    Task<Book> UpdateAsync(int id, Book entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

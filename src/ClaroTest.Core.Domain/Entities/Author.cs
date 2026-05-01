using ClaroTest.Core.Domain.Common;

namespace ClaroTest.Core.Domain.Entities;

public class Author : BaseEntity
{
    public int IdBook { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

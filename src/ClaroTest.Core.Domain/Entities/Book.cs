using ClaroTest.Core.Domain.Common;

namespace ClaroTest.Core.Domain.Entities;

public class Book : BaseEntity
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int PageCount { get; set; }
    public string? Excerpt { get; set; }
    public DateTime PublishDate { get; set; }
}

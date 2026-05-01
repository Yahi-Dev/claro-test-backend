namespace ClaroTest.Core.Application.ViewModels.Authors;

public class AuthorViewModel
{
    public int Id { get; set; }
    public int IdBook { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int BookCount { get; set; }
}

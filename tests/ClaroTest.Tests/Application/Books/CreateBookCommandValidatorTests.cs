using ClaroTest.Core.Application.Features.Books.Commands.CreateBook;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ClaroTest.Tests.Application.Books;

public class CreateBookCommandValidatorTests
{
    private readonly CreateBookCommandValidator _validator = new();

    [Fact]
    public void Title_WhenEmpty_FailsValidation()
    {
        var result = _validator.TestValidate(new CreateBookCommand { Title = string.Empty, PublishDate = DateTime.UtcNow });
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void PageCount_WhenNegative_FailsValidation()
    {
        var result = _validator.TestValidate(new CreateBookCommand { Title = "T", PageCount = -1, PublishDate = DateTime.UtcNow });
        result.ShouldHaveValidationErrorFor(x => x.PageCount);
    }

    [Fact]
    public void Valid_Command_PassesValidation()
    {
        var result = _validator.TestValidate(new CreateBookCommand
        {
            Title = "Valid Title",
            Description = "Description",
            PageCount = 100,
            Excerpt = "Excerpt",
            PublishDate = DateTime.UtcNow
        });
        result.IsValid.Should().BeTrue();
    }
}

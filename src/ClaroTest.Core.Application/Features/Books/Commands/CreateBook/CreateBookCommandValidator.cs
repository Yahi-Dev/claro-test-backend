using FluentValidation;

namespace ClaroTest.Core.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(200).WithMessage("El título no puede superar los 200 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("La descripción no puede superar los 2000 caracteres.");

        RuleFor(x => x.Excerpt)
            .MaximumLength(2000).WithMessage("El extracto no puede superar los 2000 caracteres.");

        RuleFor(x => x.PageCount)
            .GreaterThanOrEqualTo(0).WithMessage("La cantidad de páginas debe ser cero o positiva.");

        RuleFor(x => x.PublishDate)
            .NotEmpty().WithMessage("La fecha de publicación es obligatoria.");
    }
}

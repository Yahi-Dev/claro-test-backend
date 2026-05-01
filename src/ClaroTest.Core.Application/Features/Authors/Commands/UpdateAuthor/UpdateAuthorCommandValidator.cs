using FluentValidation;

namespace ClaroTest.Core.Application.Features.Authors.Commands.UpdateAuthor;

public class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El Id debe ser mayor que cero.");

        RuleFor(x => x.IdBook)
            .GreaterThan(0).WithMessage("El IdBook debe ser mayor que cero.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido no puede superar los 100 caracteres.");
    }
}

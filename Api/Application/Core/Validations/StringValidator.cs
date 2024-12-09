using FluentValidation;

namespace Application.Core.Validations;

public class StringValidator : AbstractValidator<string>
{
    public StringValidator()
    {
        RuleFor(u => u).NotNull().WithMessage("The field cannot be null.")
            .NotEmpty().WithMessage("The field should not be empty.");
        RuleSet("id-user", () =>
        {
            RuleFor(u => u).NotEmpty().WithMessage("The user login ID must not be empty.")
                .Length(36).WithMessage("Id length is not valid.");
        });
    }
}
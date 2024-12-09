using Application.Dto;
using FluentValidation;

namespace Application.Core.Validations;

public class IdeaDtoValidator : AbstractValidator<IdeaDto>
{
    public IdeaDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(255).WithMessage("Description must not exceed 255 characters.");
        RuleSet("update", () =>
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("The ID must not be empty.")
                .Length(36).WithMessage("The ID must be 36 characters long.");
            
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(255).WithMessage("Description must not exceed 255 characters.");

        });

    }

}
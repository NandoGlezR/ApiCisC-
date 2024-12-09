using Application.Dto;
using FluentValidation;

namespace Application.Core.Validations;

public class TopicDtoValidator : AbstractValidator<TopicDto>
{
    private const int MaxCharactersTitle = 255;

    public TopicDtoValidator()
    {
        RuleFor(topic => topic.Title).NotEmpty().WithMessage("The title must not be empty.")
            .MaximumLength(MaxCharactersTitle).WithMessage("The title must not be longer than 255 characters.");

        RuleSet("update", () =>
        {
            RuleFor(topic => topic.Id).NotEmpty().WithMessage("The identifier of the topic to be updated is missing.")
                .Length(36).WithMessage("Id length is not valid.");
            RuleFor(topic => topic.Title).NotEmpty().WithMessage("The title must not be empty.");
        });
    }
}
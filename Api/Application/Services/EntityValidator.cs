using Application.Interfaces;
using Domain.Core.Models;
using Domain.Exceptions;
using FluentValidation;

namespace Application.Services;

public class EntityValidator : IEntityValidator
{
    private readonly IValidator<string> _validator;

    public EntityValidator(IValidator<string> validator)
    {
        _validator = validator;
    }

    public void ValidateStringField(string field, bool isIdentifier = false)
    {
        
        var validation = isIdentifier
            ? _validator.Validate(field, options => options.IncludeRuleSets("id-user"))
            : _validator.Validate(field);

        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }
    }

    public void ValidateEntityIsNotNull(BaseEntity entity)
    {
        if (entity == null)
        {
            throw new EntityNullException();
        }
    }
}
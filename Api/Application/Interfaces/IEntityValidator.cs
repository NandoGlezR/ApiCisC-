using Domain.Core.Models;

namespace Application.Interfaces;

public interface IEntityValidator
{
    /// <summary>
    /// Validates a string field.
    /// </summary>
    /// <param name="field">The string field to validate.</param>
    /// <param name="isIdentifier">Specifies whether the field is an identifier (e.g., ID). Default is false.</param>
    /// <exception cref="ValidationException">Thrown when the validation fails.</exception>
    void ValidateStringField(string field, bool isIdentifier = false);

    /// <summary>
    /// Validates that the given entity is not null.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <exception cref="NullReferenceException">Thrown when the entity is null.</exception>
    void ValidateEntityIsNotNull(BaseEntity entity);
}
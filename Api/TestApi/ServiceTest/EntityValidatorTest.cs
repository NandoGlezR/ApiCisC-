using Application.Services;
using Domain.Core.Models;
using Domain.Exceptions;
using FluentValidation;
using Moq;

namespace TestApi.ServiceTest;

[TestFixture]
public class EntityValidatorTest
{
    private Mock<IValidator<string>> _mockValidator;
    private EntityValidator _entityValidator;

    [SetUp]
    public void SetUp()
    {
        _mockValidator = new Mock<IValidator<string>>();
        _entityValidator = new EntityValidator(_mockValidator.Object);
    }

    [Test]
    public void ValidateStringField_ShouldThrowValidationException_WhenFieldIsInvalid()
    {
        _mockValidator.Setup(validator => validator.Validate(It.IsAny<string>()))
            .Returns(new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("string", "Error")
            }));
        Assert.Throws<ValidationException>(() => _entityValidator.ValidateStringField("xc"));
    }

    [Test]
    public void ValidateStringField_ShouldNotThrowException_WhenFieldIsValid()
    {
        _mockValidator
            .Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new FluentValidation.Results.ValidationResult());
        Assert.DoesNotThrow(() => _entityValidator.ValidateStringField("valid"));
    }

    [Test]
    public void ValidateStringField_ShouldThrowValidationException_WhenIdIsInvalid()
    {
        _mockValidator
            .Setup(validator => validator.Validate(It.IsAny<ValidationContext<string>>()))
            .Returns(new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("string", "Id length is not valid.")
            }));
        
        Assert.Throws<ValidationException>(() =>
            _entityValidator.ValidateStringField("50e20a5f-d0da-41cf-b6d7", isIdentifier: true));
    }

    [Test]
    public void ValidateStringField_ShouldNotThrowException_WhenIdIsValid()
    {
        _mockValidator
            .Setup(validator => validator.Validate(It.IsAny<ValidationContext<string>>()))
            .Returns(new FluentValidation.Results.ValidationResult());
        
        Assert.DoesNotThrow(() =>
            _entityValidator.ValidateStringField("50e20a5f-d0da-41cf-b6d7-cecd44d90d38", isIdentifier: true));
    }

    [Test]
    public void ValidateEntityIsNotNull_ShouldThrowEntityNullException_WhenEntityIsNull()
    {
        Assert.Throws<EntityNullException>(() => _entityValidator.ValidateEntityIsNotNull(null));
    }

    [Test]
    public void ValidateEntityIsNotNull_ShouldNotThrowException_WhenEntityIsNotNull()
    {
        Assert.DoesNotThrow(() => _entityValidator.ValidateEntityIsNotNull(new BaseEntity
        {
            Id = "1"
        }));
    }
}
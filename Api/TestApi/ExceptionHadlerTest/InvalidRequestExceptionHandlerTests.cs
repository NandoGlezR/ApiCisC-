using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using WebApi.ExceptionHandler;

namespace TestApi.ExceptionHadlerTest;

[TestFixture]
public class InvalidRequestExceptionHandlerTests
{
    private InvalidRequestExceptionHandler _exceptionHandler;

    [SetUp]
    public void SetUp()
    {
        _exceptionHandler = new InvalidRequestExceptionHandler();
    }

    [Test]
    public async Task TryHandleAsync_ShouldHandleValidationExceptionAndSetResponse()
    {
        var validationFailure = new ValidationFailure("Field", "Validation error message");
        var validationException = new ValidationException(new[] { validationFailure });
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        
        var result = await _exceptionHandler.TryHandleAsync(httpContext, validationException, default);

        Assert.IsTrue(result);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

    }

    [Test]
    public async Task TryHandleAsync_ShouldReturnFalse_WhenExceptionIsNotValidationException()
    {
        var exception = new Exception("Some other exception");
        var httpContext = new DefaultHttpContext();

        var result = await _exceptionHandler.TryHandleAsync(httpContext, exception, default);
        
        Assert.IsFalse(result);
    }
}

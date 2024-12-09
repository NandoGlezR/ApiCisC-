using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using WebApi.ExceptionHandler;

namespace TestApi.ExceptionHadlerTest;


[TestFixture]
public class EntityNullExceptionHandlerTests
{
    private EntityNullExceptionHandler _exceptionHandler;

    [SetUp]
    public void SetUp()
    {
        _exceptionHandler = new EntityNullExceptionHandler();
    }

    [Test]
    public async Task TryHandleAsync_ShouldHandleEntityNullExceptionAndSetResponse()
    {
        var exception = new EntityNullException("Entity not found");
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        
        var result = await _exceptionHandler.TryHandleAsync(httpContext, exception, default);
        
        Assert.IsTrue(result);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        
    }

    [Test]
    public async Task TryHandleAsync_ShouldReturnFalse_WhenExceptionIsNotEntityNullException()
    {
        var exception = new Exception("Some other exception");
        var httpContext = new DefaultHttpContext();

        var result = await _exceptionHandler.TryHandleAsync(httpContext, exception, default);

        Assert.IsFalse(result);
    }
}
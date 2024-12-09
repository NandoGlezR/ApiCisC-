using Microsoft.AspNetCore.Http;
using WebApi.ExceptionHandler;
using WebApi.Security.Exceptions;

namespace TestApi.ExceptionHadlerTest;

[TestFixture]
public class AuthorizationExceptionHandlerTest
{
    private AuthorizationExceptionHandler _exceptionHandler;

    [SetUp]
    public void Setup()
    {
        _exceptionHandler = new AuthorizationExceptionHandler();
    }
    
    [Test]
    public async Task TryHandleAsync_ShouldHandleEntityNullExceptionAndSetResponse()
    {
        var exception = new AuthorizationException("You are not allowed to access this resource");
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        
        var result = await _exceptionHandler.TryHandleAsync(httpContext, exception, default);
        
        Assert.IsTrue(result);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
        
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
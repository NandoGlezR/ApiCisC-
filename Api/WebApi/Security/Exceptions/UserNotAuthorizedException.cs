namespace WebApi.Security.Exceptions;

public class UserNotAuthorizedException : AuthorizationException
{
    public UserNotAuthorizedException(string message) : base(message) {}
}
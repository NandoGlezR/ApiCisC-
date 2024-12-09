using System.Diagnostics.CodeAnalysis;

namespace WebApi.Security.Exceptions;

[ExcludeFromCodeCoverage]
public class AuthorizationException : Exception
{
    public AuthorizationException(string message) : base(message){}

    public AuthorizationException() : base("You are not allowed to access this resource") {}
}
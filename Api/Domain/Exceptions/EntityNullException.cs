
namespace Domain.Exceptions;

/// <summary>
/// This exception is focused on resolving conflicts of requests to the server that did not return a value.
/// </summary>
public class EntityNullException : Exception
{
    public EntityNullException(string message) : base(message) { }
    public EntityNullException() : base("The requested entity is not available.") { }
}
namespace Nexora.Management.Application.Common;

/// <summary>
/// Base exception class for application-specific exceptions
/// </summary>
public abstract class AppException : Exception
{
    public AppException(string message) : base(message) { }
    public AppException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string name, object key) : base($"Entity '{name}' ({key}) was not found.") { }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : AppException
{
    public List<string> Errors { get; }

    public ValidationException(List<string> errors) : base("Validation failed")
    {
        Errors = errors;
    }

    public ValidationException(string error) : base("Validation failed")
    {
        Errors = new List<string> { error };
    }
}

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessRuleException : AppException
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when an unauthorized operation is attempted
/// </summary>
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message) { }
    public UnauthorizedException() : base("Unauthorized access") { }
}

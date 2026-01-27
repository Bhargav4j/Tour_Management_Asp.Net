namespace TourManagement.Domain.Exceptions;

/// <summary>
/// Base exception for tour management domain
/// </summary>
public class TourManagementException : Exception
{
    public TourManagementException() : base() { }
    public TourManagementException(string message) : base(message) { }
    public TourManagementException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : TourManagementException
{
    public EntityNotFoundException() : base() { }
    public EntityNotFoundException(string message) : base(message) { }
    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : TourManagementException
{
    public ValidationException() : base() { }
    public ValidationException(string message) : base(message) { }
    public ValidationException(string message, Exception innerException) : base(message, innerException) { }
}

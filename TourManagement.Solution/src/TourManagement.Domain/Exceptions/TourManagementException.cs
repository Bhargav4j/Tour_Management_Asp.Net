namespace TourManagement.Domain.Exceptions;

/// <summary>
/// Base exception for tour management domain
/// </summary>
public class TourManagementException : Exception
{
    public TourManagementException() { }
    public TourManagementException(string message) : base(message) { }
    public TourManagementException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when entity is not found
/// </summary>
public class EntityNotFoundException : TourManagementException
{
    public EntityNotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' with key '{key}' was not found.") { }
}

/// <summary>
/// Exception thrown when entity already exists
/// </summary>
public class EntityAlreadyExistsException : TourManagementException
{
    public EntityAlreadyExistsException(string entityName, object key)
        : base($"Entity '{entityName}' with key '{key}' already exists.") { }
}

/// <summary>
/// Exception thrown for validation errors
/// </summary>
public class ValidationException : TourManagementException
{
    public ValidationException(string message) : base(message) { }
}

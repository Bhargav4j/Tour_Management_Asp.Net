namespace TourManagement.Domain.Exceptions;

/// <summary>
/// Exception thrown for business rule violations
/// </summary>
public class BusinessException : Exception
{
    public BusinessException(string message)
        : base(message)
    {
    }

    public BusinessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

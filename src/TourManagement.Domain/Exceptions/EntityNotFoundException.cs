namespace TourManagement.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, int id)
        : base($"{entityName} with ID {id} was not found.")
    {
        EntityName = entityName;
        EntityId = id;
    }

    public string EntityName { get; }
    public int EntityId { get; }
}

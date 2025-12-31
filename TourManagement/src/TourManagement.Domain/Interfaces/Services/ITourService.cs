namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Tour business operations
/// </summary>
public interface ITourService
{
    Task<IEnumerable<TourDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TourDto> CreateAsync(TourCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, TourUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<TourDto>> GetByPlaceAsync(string place, CancellationToken cancellationToken = default);
}

public class TourDto
{
    public int Id { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class TourCreateDto
{
    public string TourName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class TourUpdateDto
{
    public string TourName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

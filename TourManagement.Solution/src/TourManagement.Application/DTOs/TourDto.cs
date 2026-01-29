namespace TourManagement.Application.DTOs;

/// <summary>
/// Data transfer object for Tour entity
/// </summary>
public class TourDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string Locations { get; set; } = string.Empty;
    public string TourInfo { get; set; } = string.Empty;
    public string? PicturePath { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating a new tour
/// </summary>
public class TourCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string Locations { get; set; } = string.Empty;
    public string TourInfo { get; set; } = string.Empty;
    public string? PicturePath { get; set; }
}

/// <summary>
/// DTO for updating a tour
/// </summary>
public class TourUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string Locations { get; set; } = string.Empty;
    public string TourInfo { get; set; } = string.Empty;
    public string? PicturePath { get; set; }
    public bool IsActive { get; set; }
}

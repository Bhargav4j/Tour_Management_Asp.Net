namespace TourManagement.Domain.DTOs;

public class TourDto
{
    public int Id { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string Locations { get; set; } = string.Empty;
    public string TourInfo { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

public class TourCreateDto
{
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string Locations { get; set; } = string.Empty;
    public string TourInfo { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
    public string CreatedBy { get; set; } = "System";
}

public class TourUpdateDto
{
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string Locations { get; set; } = string.Empty;
    public string TourInfo { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
    public string ModifiedBy { get; set; } = "System";
}

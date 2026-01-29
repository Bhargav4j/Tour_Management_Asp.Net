namespace TourManagement.Application.DTOs;

/// <summary>
/// Data transfer object for Booking entity
/// </summary>
public class BookingDto
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating a new booking
/// </summary>
public class BookingCreateDto
{
    public int TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
}

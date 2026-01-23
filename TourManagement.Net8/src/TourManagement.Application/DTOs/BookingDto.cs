namespace TourManagement.Application.DTOs;

/// <summary>
/// Data transfer object for Booking entity
/// </summary>
public class BookingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TourId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string TourName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public int NumberOfPersons { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating a new booking
/// </summary>
public class BookingCreateDto
{
    public int UserId { get; set; }
    public int TourId { get; set; }
    public DateTime BookingDate { get; set; }
    public int NumberOfPersons { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// DTO for updating an existing booking
/// </summary>
public class BookingUpdateDto
{
    public DateTime BookingDate { get; set; }
    public int NumberOfPersons { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
}

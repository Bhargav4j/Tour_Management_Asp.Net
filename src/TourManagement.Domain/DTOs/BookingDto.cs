namespace TourManagement.Domain.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

public class BookingCreateDto
{
    public int TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string CreatedBy { get; set; } = "System";
}

public class BookingUpdateDto
{
    public DateTime BookingDate { get; set; }
    public string ModifiedBy { get; set; } = "System";
}

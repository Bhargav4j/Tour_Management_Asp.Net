namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a booking entity in the system
/// </summary>
public class Booking
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public int UserId { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public int NumberOfPeople { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = "System";
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public Tour Tour { get; set; } = null!;
    public User User { get; set; } = null!;
}

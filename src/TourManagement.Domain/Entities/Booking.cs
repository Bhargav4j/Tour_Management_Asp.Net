namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour booking in the system
/// </summary>
public class Booking
{
    public int BookingId { get; set; }
    public int TourId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = "Pending";

    public Tour Tour { get; set; } = null!;
    public User User { get; set; } = null!;
}

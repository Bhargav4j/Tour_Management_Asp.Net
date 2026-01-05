namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour booking in the system
/// </summary>
public class Booking
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public int TourId { get; set; }
    public DateTime BookingDate { get; set; }
    public int NumberOfPeople { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Tour Tour { get; set; } = null!;
}

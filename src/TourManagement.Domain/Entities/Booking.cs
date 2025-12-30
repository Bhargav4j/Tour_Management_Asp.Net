namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour booking made by a user
/// </summary>
public class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int TourId { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    public int NumberOfPeople { get; set; } = 1;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; } = true;

    public string CreatedBy { get; set; } = string.Empty;

    public string? ModifiedBy { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}

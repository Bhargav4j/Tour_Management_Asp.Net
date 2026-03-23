namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a booking entity
/// </summary>
public class Booking
{
    public int BookingId { get; set; }
    public int TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = "system";
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public virtual Tour? Tour { get; set; }
    public virtual UserInfo? User { get; set; }
}

namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour package in the system
/// </summary>
public class Tour
{
    public int TourId { get; set; }

    public string TourName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Days { get; set; }

    public string? Locations { get; set; }

    public string? Picture { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; } = true;

    public string CreatedBy { get; set; } = string.Empty;

    public string? ModifiedBy { get; set; }

    // Navigation properties
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour booking
/// </summary>
public class Booking
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public int UserId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = "System";
    public string? ModifiedBy { get; set; }

    public virtual Tour? Tour { get; set; }
    public virtual User? User { get; set; }
}

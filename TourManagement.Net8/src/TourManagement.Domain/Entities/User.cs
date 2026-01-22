namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsAdmin { get; set; } = false;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = "System";
    public string? ModifiedBy { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagement.Web.Models;

[Table("user_info")]
public class UserInfo
{
    [Key]
    [Column("email")]
    [MaxLength(50)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("first_name")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Column("last_name")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Column("gender")]
    [MaxLength(10)]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [Column("password")]
    [MaxLength(50)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Column("dob")]
    public DateOnly Dob { get; set; }

    [Required]
    [Column("street")]
    [MaxLength(50)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [Column("city")]
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    [Required]
    [Column("state")]
    [MaxLength(50)]
    public string State { get; set; } = string.Empty;
}

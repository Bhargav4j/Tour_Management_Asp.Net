using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagement.Web.Models;

[Table("booking")]
public class Booking
{
    [Key]
    [Column("tour_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TourId { get; set; }

    [Column("tour_name")]
    [MaxLength(50)]
    public string? TourName { get; set; }

    [Column("place")]
    [MaxLength(50)]
    public string? Place { get; set; }

    [Column("email")]
    [MaxLength(50)]
    public string? Email { get; set; }

    [Column("first_name")]
    [MaxLength(50)]
    public string? FirstName { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagement.Web.Models;

[Table("tour")]
public class Tour
{
    [Key]
    [Column("tour_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TourId { get; set; }

    [Required]
    [Column("tour_name")]
    [MaxLength(20)]
    public string TourName { get; set; } = string.Empty;

    [Required]
    [Column("place")]
    [MaxLength(20)]
    public string Place { get; set; } = string.Empty;

    [Required]
    [Column("days")]
    public short Days { get; set; }

    [Required]
    [Column("price")]
    public decimal Price { get; set; }

    [Required]
    [Column("locations")]
    [MaxLength(100)]
    public string Locations { get; set; } = string.Empty;

    [Required]
    [Column("tour_info")]
    [MaxLength(200)]
    public string TourInfo { get; set; } = string.Empty;

    [Column("pic")]
    [MaxLength(200)]
    public string? Pic { get; set; }
}

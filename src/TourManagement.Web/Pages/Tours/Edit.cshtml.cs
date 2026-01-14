using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        ITourService tourService,
        IWebHostEnvironment environment,
        ILogger<EditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public int Id { get; set; }

    [BindProperty]
    [Required]
    [StringLength(20)]
    public string TourName { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [StringLength(20)]
    public string Place { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Range(1, 99)]
    public int Days { get; set; }

    [BindProperty]
    [Required]
    [Range(0, 999999)]
    public decimal Price { get; set; }

    [BindProperty]
    [Required]
    [StringLength(100)]
    public string Locations { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [StringLength(200)]
    public string TourInfo { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

    public string? CurrentPictureUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var tour = await _tourService.GetByIdAsync(id, cancellationToken);

            if (tour == null)
            {
                return NotFound();
            }

            Id = tour.Id;
            TourName = tour.TourName;
            Place = tour.Place;
            Days = tour.Days;
            Price = tour.Price;
            Locations = tour.Locations;
            TourInfo = tour.TourInfo;
            CurrentPictureUrl = tour.PictureUrl;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit, id {TourId}", id);
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var existingTour = await _tourService.GetByIdAsync(Id, cancellationToken);
            if (existingTour == null)
            {
                return NotFound();
            }

            string? pictureFileName = existingTour.PictureUrl;

            if (PictureFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);

                pictureFileName = Guid.NewGuid().ToString() + Path.GetExtension(PictureFile.FileName);
                var filePath = Path.Combine(uploadsFolder, pictureFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(fileStream, cancellationToken);
                }

                if (!string.IsNullOrEmpty(existingTour.PictureUrl))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, existingTour.PictureUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }

            var dto = new TourUpdateDto
            {
                TourName = TourName,
                Place = Place,
                Days = Days,
                Price = Price,
                Locations = Locations,
                TourInfo = TourInfo,
                PictureUrl = pictureFileName,
                ModifiedBy = User.Identity?.Name ?? "System"
            };

            await _tourService.UpdateAsync(Id, dto, cancellationToken);

            TempData["SuccessMessage"] = "Tour updated successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with id {TourId}", Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour.");
            return Page();
        }
    }
}

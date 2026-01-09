using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
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
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Place { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Range(1, 365)]
    public int Days { get; set; }

    [BindProperty]
    [Required]
    [Range(0, 1000000)]
    public decimal Price { get; set; }

    [BindProperty]
    [Required]
    public string Locations { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Display(Name = "Tour Information")]
    public string TourInfo { get; set; } = string.Empty;

    [BindProperty]
    [Display(Name = "Tour Image")]
    public IFormFile? ImageFile { get; set; }

    [BindProperty]
    public string? CurrentImageFileName { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken)
    {
        if (id == null)
        {
            return NotFound();
        }

        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            var tour = await _tourService.GetTourByIdAsync(id.Value, cancellationToken);

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
            CurrentImageFileName = tour.ImageFileName;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit");
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            var existingTour = await _tourService.GetTourByIdAsync(Id, cancellationToken);
            if (existingTour == null)
            {
                return NotFound();
            }

            existingTour.TourName = TourName;
            existingTour.Place = Place;
            existingTour.Days = Days;
            existingTour.Price = Price;
            existingTour.Locations = Locations;
            existingTour.TourInfo = TourInfo;
            existingTour.ModifiedBy = HttpContext.Session.GetString("UserEmail") ?? "Admin";
            existingTour.ModifiedDate = DateTime.UtcNow;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadPath = Environment.GetEnvironmentVariable("UPLOAD_PATH") ?? Path.Combine(_environment.WebRootPath, "images", "tours");
                var uploadsFolder = uploadPath;
                Directory.CreateDirectory(uploadsFolder);

                if (!string.IsNullOrEmpty(existingTour.ImageFileName))
                {
                    var oldImagePath = Path.Combine(uploadsFolder, existingTour.ImageFileName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream, cancellationToken);
                }

                existingTour.ImageFileName = uniqueFileName;
            }

            await _tourService.UpdateTourAsync(existingTour, cancellationToken);

            _logger.LogInformation("Tour updated: {TourId}", Id);
            TempData["SuccessMessage"] = "Tour updated successfully!";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour");
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour.");
            return Page();
        }
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    public EditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<EditModel> logger)
    {
        _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public TourInputModel TourInput { get; set; } = new();

    public class TourInputModel
    {
        public int TourId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Tour Name")]
        public string TourName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Place { get; set; } = string.Empty;

        [Required]
        [Range(1, 99)]
        public int Days { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(100)]
        public string Locations { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Tour Information")]
        public string TourInfo { get; set; } = string.Empty;

        [Display(Name = "Tour Image")]
        public IFormFile? ImageFile { get; set; }

        public string? CurrentImageFileName { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var tour = await _tourService.GetByIdAsync(id, cancellationToken);

            if (tour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
                return NotFound();
            }

            // Map entity to input model
            TourInput = new TourInputModel
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price,
                Locations = tour.Locations,
                TourInfo = tour.TourInfo,
                CurrentImageFileName = tour.Pic
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit: {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the tour.";
            return RedirectToPage("./Index");
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
            var existingTour = await _tourService.GetByIdAsync(TourInput.TourId, cancellationToken);
            if (existingTour == null)
            {
                return NotFound();
            }

            string? fileName = TourInput.CurrentImageFileName;

            // Handle new file upload
            if (TourInput.ImageFile != null && TourInput.ImageFile.Length > 0)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(TourInput.ImageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("TourInput.ImageFile", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                    return Page();
                }

                // Validate file size (5MB max)
                if (TourInput.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("TourInput.ImageFile", "File size must not exceed 5MB.");
                    return Page();
                }

                // Delete old image if exists
                if (!string.IsNullOrEmpty(TourInput.CurrentImageFileName))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, "images", TourInput.CurrentImageFileName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                fileName = $"{Guid.NewGuid()}{extension}";
                var imagesPath = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                var filePath = Path.Combine(imagesPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await TourInput.ImageFile.CopyToAsync(stream, cancellationToken);
                }

                _logger.LogInformation("Uploaded new image file: {FileName}", fileName);
            }

            // Map input to entity
            var tour = new Tour
            {
                TourId = TourInput.TourId,
                TourName = TourInput.TourName,
                Place = TourInput.Place,
                Days = TourInput.Days,
                Price = TourInput.Price,
                Locations = TourInput.Locations,
                TourInfo = TourInput.TourInfo,
                Pic = fileName,
                IsActive = true,
                ModifiedBy = "admin"
            };

            await _tourService.UpdateAsync(TourInput.TourId, tour, cancellationToken);

            _logger.LogInformation("Updated tour: {TourId}", TourInput.TourId);
            TempData["SuccessMessage"] = "Tour updated successfully!";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour: {TourId}", TourInput.TourId);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour.");
            return Page();
        }
    }
}

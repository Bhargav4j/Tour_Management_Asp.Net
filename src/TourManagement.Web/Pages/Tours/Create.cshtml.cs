using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for creating a new tour
/// </summary>
public class CreateModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        ITourService tourService,
        IWebHostEnvironment environment,
        ILogger<CreateModel> logger)
    {
        _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public TourViewModel Tour { get; set; } = new TourViewModel();

    public IActionResult OnGet()
    {
        // Initialize with default values
        Tour.IsActive = true;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Handle image upload
            string? fileName = null;
            if (Tour.ImageFile != null)
            {
                fileName = await SaveImageAsync(Tour.ImageFile, cancellationToken);
            }

            // Map ViewModel to Entity
            var tourEntity = new Tour
            {
                TourName = Tour.TourName,
                Place = Tour.Place,
                Days = Tour.Days,
                Price = Tour.Price,
                Locations = Tour.Locations ?? string.Empty,
                TourInfo = Tour.TourInfo ?? string.Empty,
                PictureFileName = fileName,
                IsActive = Tour.IsActive,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            var createdTour = await _tourService.CreateAsync(tourEntity, cancellationToken);

            _logger.LogInformation("Tour created successfully with ID: {TourId}", createdTour.Id);
            TempData["SuccessMessage"] = $"Tour '{createdTour.TourName}' created successfully!";

            return RedirectToPage("./Index");
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error while creating tour");
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the tour. Please try again.");
            return Page();
        }
    }

    /// <summary>
    /// Saves the uploaded image to the wwwroot/images/tours directory
    /// </summary>
    private async Task<string?> SaveImageAsync(IFormFile imageFile, CancellationToken cancellationToken)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return null;
        }

        try
        {
            // Validate file size (5MB max)
            if (imageFile.Length > 5242880)
            {
                ModelState.AddModelError("Tour.ImageFile", "File size must be less than 5MB");
                return null;
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("Tour.ImageFile", "Invalid file type. Only JPG, PNG, and GIF files are allowed.");
                return null;
            }

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "tours");

            // Ensure directory exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream, cancellationToken);
            }

            _logger.LogInformation("Image saved successfully: {FileName}", uniqueFileName);
            return uniqueFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving image file");
            ModelState.AddModelError("Tour.ImageFile", "An error occurred while saving the image. Please try again.");
            return null;
        }
    }
}

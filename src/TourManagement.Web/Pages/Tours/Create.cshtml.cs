using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tours;

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
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public TourInputModel Input { get; set; } = new TourInputModel();

    public void OnGet()
    {
        // Initialize with default values
        Input = new TourInputModel
        {
            IsActive = true
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            string? pictureFileName = null;

            // Handle file upload
            if (Input.ImageFile != null)
            {
                pictureFileName = await SaveImageFileAsync(Input.ImageFile);
            }

            // Manual mapping from InputModel to CreateDto
            var createDto = new TourCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                PictureFileName = pictureFileName
            };

            var createdTour = await _tourService.CreateAsync(createDto);

            _logger.LogInformation("Tour created successfully with ID: {TourId}", createdTour.Id);
            TempData["SuccessMessage"] = $"Tour '{createdTour.TourName}' created successfully.";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour");
            TempData["ErrorMessage"] = "An error occurred while creating the tour. Please try again.";
            return Page();
        }
    }

    private async Task<string> SaveImageFileAsync(IFormFile imageFile)
    {
        try
        {
            // Validate file size (5MB limit)
            if (imageFile.Length > 5 * 1024 * 1024)
            {
                throw new InvalidOperationException("File size exceeds 5MB limit.");
            }

            // Validate file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Invalid file format. Only JPG, PNG, and GIF are allowed.");
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "tours");

            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            _logger.LogInformation("Image saved successfully: {FileName}", fileName);
            return fileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving image file");
            throw;
        }
    }

    public class TourInputModel
    {
        [Required(ErrorMessage = "Tour name is required")]
        [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
        public string TourName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Place is required")]
        [StringLength(100, ErrorMessage = "Place cannot exceed 100 characters")]
        public string Place { get; set; } = string.Empty;

        [Required(ErrorMessage = "Days is required")]
        [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
        public int Days { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Locations is required")]
        [StringLength(500, ErrorMessage = "Locations cannot exceed 500 characters")]
        public string Locations { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tour information is required")]
        [StringLength(2000, ErrorMessage = "Tour information cannot exceed 2000 characters")]
        public string TourInfo { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

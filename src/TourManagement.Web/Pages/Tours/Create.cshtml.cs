using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class CreateModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CreateModel> _logger;
    private readonly IConfiguration _configuration;

    public CreateModel(ITourService tourService, IWebHostEnvironment environment, ILogger<CreateModel> logger, IConfiguration configuration)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
        _configuration = configuration;
    }

    [BindProperty]
    [Required(ErrorMessage = "Tour name is required")]
    public string TourName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Place is required")]
    public string Place { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    public int Days { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [BindProperty]
    public string Locations { get; set; } = string.Empty;

    [BindProperty]
    public string TourInfo { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        _logger.LogInformation("Create tour page accessed");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            string? picturePath = null;

            if (PictureFile != null && PictureFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(PictureFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ErrorMessage = "Invalid file type. Only images are allowed.";
                    return Page();
                }

                if (PictureFile.Length > 5 * 1024 * 1024)
                {
                    ErrorMessage = "File size must not exceed 5MB.";
                    return Page();
                }

                // Get upload path from configuration (supports persistent volumes or cloud storage)
                var uploadsFolder = _configuration.GetValue<string>("FileStorage:UploadPath");
                if (string.IsNullOrEmpty(uploadsFolder))
                {
                    uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                }
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(stream);
                }

                picturePath = uniqueFileName;
            }

            var tour = new Tour
            {
                TourName = TourName,
                Place = Place,
                Days = Days,
                Price = Price,
                Locations = Locations,
                TourInfo = TourInfo,
                PicturePath = picturePath
            };

            await _tourService.CreateAsync(tour);

            _logger.LogInformation("Tour {TourName} created successfully", TourName);

            TempData["SuccessMessage"] = "Tour created successfully!";
            return RedirectToPage("/Tours/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour");
            ErrorMessage = "An error occurred while creating the tour. Please try again.";
            return Page();
        }
    }
}

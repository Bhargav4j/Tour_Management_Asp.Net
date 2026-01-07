using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class CreateTourModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CreateTourModel> _logger;

    public CreateTourModel(ITourService tourService, IWebHostEnvironment environment, ILogger<CreateTourModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

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
    [Range(0.01, 999999.99)]
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
    public IFormFile? ImageFile { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            string? imageFileName = null;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ErrorMessage = "Only image files (.jpg, .jpeg, .png, .gif) are allowed.";
                    return Page();
                }

                if (ImageFile.Length > 5 * 1024 * 1024)
                {
                    ErrorMessage = "File size must be less than 5 MB.";
                    return Page();
                }

                imageFileName = $"{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine("/app/wwwroot/uploads", "tours");
                Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, imageFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
            }

            var tour = new Tour
            {
                TourName = TourName,
                Place = Place,
                Days = Days,
                Price = Price,
                Locations = Locations,
                TourInfo = TourInfo,
                ImageFileName = imageFileName
            };

            await _tourService.CreateTourAsync(tour);

            _logger.LogInformation("Tour {TourName} created successfully", TourName);

            return RedirectToPage("/Tours/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", TourName);
            ErrorMessage = "An error occurred while creating the tour. Please try again.";
            return Page();
        }
    }
}

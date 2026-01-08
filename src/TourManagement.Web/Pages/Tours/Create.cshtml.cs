using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

[Authorize]
public class TourCreateModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<TourCreateModel> _logger;

    public TourCreateModel(ITourService tourService, IWebHostEnvironment environment, ILogger<TourCreateModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(200)]
    public string TourName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Place is required")]
    [StringLength(200)]
    public string Place { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    public int Days { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Locations is required")]
    [StringLength(500)]
    public string Locations { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Tour information is required")]
    [StringLength(2000)]
    public string TourInfo { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
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
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Tour_pics");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{PictureFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(fileStream, cancellationToken);
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

            await _tourService.CreateTourAsync(tour, cancellationToken);

            _logger.LogInformation("Tour created successfully: {TourName}", TourName);

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

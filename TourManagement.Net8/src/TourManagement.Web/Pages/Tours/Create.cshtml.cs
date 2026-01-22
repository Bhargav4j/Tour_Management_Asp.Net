using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class CreateModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(ITourService tourService, IWebHostEnvironment environment, ILogger<CreateModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
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
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Locations is required")]
    public string Locations { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Tour information is required")]
    public string TourInfo { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

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
            var tour = new Tour
            {
                TourName = TourName,
                Place = Place,
                Days = Days,
                Price = Price,
                Locations = Locations,
                TourInfo = TourInfo,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "Admin",
                IsActive = true
            };

            if (PictureFile != null && PictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Tour_pics");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + PictureFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(fileStream);
                }

                tour.PicturePath = uniqueFileName;
            }

            await _tourService.CreateTourAsync(tour);

            TempData["SuccessMessage"] = "Tour created successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the tour.");
            return Page();
        }
    }
}

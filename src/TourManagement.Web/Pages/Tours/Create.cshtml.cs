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
    public TourInputModel TourInput { get; set; } = new();

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
                Name = TourInput.Name,
                Place = TourInput.Place,
                Days = TourInput.Days,
                Price = TourInput.Price,
                Locations = TourInput.Locations,
                TourInfo = TourInput.TourInfo
            };

            // Handle file upload
            if (TourInput.PictureFile != null && TourInput.PictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{TourInput.PictureFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await TourInput.PictureFile.CopyToAsync(fileStream);
                }

                tour.PicturePath = uniqueFileName;
            }

            await _tourService.CreateTourAsync(tour);

            _logger.LogInformation("Created tour: {TourName}", tour.Name);
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

    public class TourInputModel
    {
        [Required(ErrorMessage = "Tour name is required")]
        [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Place is required")]
        [StringLength(200, ErrorMessage = "Place cannot exceed 200 characters")]
        public string Place { get; set; } = string.Empty;

        [Required(ErrorMessage = "Days is required")]
        [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
        public int Days { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Locations is required")]
        [StringLength(500, ErrorMessage = "Locations cannot exceed 500 characters")]
        public string Locations { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Tour info cannot exceed 250 characters")]
        public string? TourInfo { get; set; }

        [Display(Name = "Picture")]
        public IFormFile? PictureFile { get; set; }
    }
}

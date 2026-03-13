using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class CreateModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<CreateModel> _logger;
    private readonly IWebHostEnvironment _environment;

    public CreateModel(ITourService tourService, ILogger<CreateModel> logger, IWebHostEnvironment environment)
    {
        _tourService = tourService;
        _logger = logger;
        _environment = environment;
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
                TourName = TourInput.TourName,
                Place = TourInput.Place,
                Days = TourInput.Days,
                Price = TourInput.Price,
                Locations = TourInput.Locations,
                TourInfo = TourInput.TourInfo,
                CreatedBy = "Admin",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            if (TourInput.Picture != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "tours");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{TourInput.Picture.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await TourInput.Picture.CopyToAsync(fileStream);
                }

                tour.PicturePath = uniqueFileName;
            }

            await _tourService.CreateAsync(tour);

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
        [Required]
        [StringLength(200)]
        [Display(Name = "Tour Name")]
        public string TourName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Place { get; set; } = string.Empty;

        [Required]
        [Range(1, 365)]
        public int Days { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(500)]
        public string Locations { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Tour Information")]
        public string TourInfo { get; set; } = string.Empty;

        [Display(Name = "Picture")]
        public IFormFile? Picture { get; set; }
    }
}

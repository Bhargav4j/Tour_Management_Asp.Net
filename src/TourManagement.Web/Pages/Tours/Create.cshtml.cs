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
    public TourInputModel Input { get; set; } = new TourInputModel();

    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

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
            string? fileName = null;

            if (Input.PictureFile != null)
            {
                fileName = await SaveFileAsync(Input.PictureFile);
            }

            var tour = new Tour
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                PictureFileName = fileName,
                CreatedBy = "Admin",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _tourService.CreateTourAsync(tour);

            _logger.LogInformation("Tour created successfully: {TourName}", tour.TourName);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour");
            Message = "Error creating tour. Please try again.";
            IsSuccess = false;
            return Page();
        }
    }

    private async Task<string> SaveFileAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "Tour_pics");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return uniqueFileName;
    }

    public class TourInputModel
    {
        [Required]
        [StringLength(200)]
        public string TourName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Place { get; set; } = string.Empty;

        [Required]
        [Range(1, 365)]
        public int Days { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(500)]
        public string Locations { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string TourInfo { get; set; } = string.Empty;

        public IFormFile? PictureFile { get; set; }
    }
}

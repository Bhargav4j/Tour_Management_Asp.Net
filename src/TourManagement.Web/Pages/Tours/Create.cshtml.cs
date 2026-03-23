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

    public CreateModel(ITourService tourService, IWebHostEnvironment environment, ILogger<CreateModel> logger)
    {
        _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public TourInputModel TourInput { get; set; } = new();

    public class TourInputModel
    {
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
    }

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
            string? fileName = null;

            // Handle file upload
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

                // Generate unique file name
                fileName = $"{Guid.NewGuid()}{extension}";
                var imagesPath = Path.Combine(_environment.WebRootPath, "images");

                // Ensure directory exists
                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                var filePath = Path.Combine(imagesPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await TourInput.ImageFile.CopyToAsync(stream, cancellationToken);
                }

                _logger.LogInformation("Uploaded image file: {FileName}", fileName);
            }

            // Map input to entity
            var tour = new Tour
            {
                TourName = TourInput.TourName,
                Place = TourInput.Place,
                Days = TourInput.Days,
                Price = TourInput.Price,
                Locations = TourInput.Locations,
                TourInfo = TourInput.TourInfo,
                Pic = fileName,
                CreatedBy = "admin"
            };

            await _tourService.CreateAsync(tour, cancellationToken);

            _logger.LogInformation("Created new tour: {TourName}", tour.TourName);
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tours;

public class CreateTourModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CreateTourModel> _logger;

    public CreateTourModel(
        ITourService tourService,
        IWebHostEnvironment environment,
        ILogger<CreateTourModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public TourInputModel Input { get; set; } = new TourInputModel();

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
            string? fileName = null;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
            }

            var createDto = new TourCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                Pic = fileName
            };

            await _tourService.CreateAsync(createDto);

            _logger.LogInformation("Tour {TourName} created successfully", Input.TourName);
            return RedirectToPage("/Tours/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour {TourName}", Input.TourName);
            ErrorMessage = "An error occurred while creating the tour.";
            return Page();
        }
    }

    public class TourInputModel
    {
        [Required(ErrorMessage = "Tour name is required")]
        [StringLength(20, ErrorMessage = "Tour name cannot exceed 20 characters")]
        public string TourName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Place is required")]
        [StringLength(20, ErrorMessage = "Place cannot exceed 20 characters")]
        public string Place { get; set; } = string.Empty;

        [Required(ErrorMessage = "Number of days is required")]
        [Range(1, 99, ErrorMessage = "Days must be between 1 and 99")]
        public int Days { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 999999, ErrorMessage = "Price must be between 0 and 999999")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Locations are required")]
        [StringLength(100, ErrorMessage = "Locations cannot exceed 100 characters")]
        public string Locations { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tour information is required")]
        [StringLength(200, ErrorMessage = "Tour information cannot exceed 200 characters")]
        public string TourInfo { get; set; } = string.Empty;
    }
}

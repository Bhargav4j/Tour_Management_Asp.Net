using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tours;

public class CreateModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<CreateModel> _logger;
    private readonly IWebHostEnvironment _environment;

    public CreateModel(
        ITourService tourService,
        ILogger<CreateModel> logger,
        IWebHostEnvironment environment)
    {
        _tourService = tourService;
        _logger = logger;
        _environment = environment;
    }

    [BindProperty]
    public TourInputModel TourInput { get; set; } = new();

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
            var createDto = new TourCreateDto
            {
                TourName = TourInput.TourName,
                Place = TourInput.Place,
                Days = TourInput.Days,
                Price = TourInput.Price,
                Locations = TourInput.Locations,
                TourInfo = TourInput.TourInfo
            };

            if (PictureFile != null && PictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "tours");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{PictureFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(fileStream);
                }

                createDto.PictureFileName = uniqueFileName;
            }

            await _tourService.CreateAsync(createDto);

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
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [StringLength(500)]
    public string Locations { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string TourInfo { get; set; } = string.Empty;
}

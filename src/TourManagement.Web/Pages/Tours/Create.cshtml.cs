using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.DTOs;
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
    [Range(0, 999999)]
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
    public IFormFile? PictureFile { get; set; }

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
            string? pictureFileName = null;

            if (PictureFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);

                pictureFileName = Guid.NewGuid().ToString() + Path.GetExtension(PictureFile.FileName);
                var filePath = Path.Combine(uploadsFolder, pictureFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(fileStream, cancellationToken);
                }
            }

            var dto = new TourCreateDto
            {
                TourName = TourName,
                Place = Place,
                Days = Days,
                Price = Price,
                Locations = Locations,
                TourInfo = TourInfo,
                PictureUrl = pictureFileName,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            await _tourService.CreateAsync(dto, cancellationToken);

            TempData["SuccessMessage"] = "Tour created successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the tour.");
            return Page();
        }
    }
}

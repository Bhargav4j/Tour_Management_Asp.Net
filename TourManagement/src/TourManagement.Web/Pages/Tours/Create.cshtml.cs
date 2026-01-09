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
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Place { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Range(1, 365)]
    public int Days { get; set; }

    [BindProperty]
    [Required]
    [Range(0, 1000000)]
    public decimal Price { get; set; }

    [BindProperty]
    [Required]
    public string Locations { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Display(Name = "Tour Information")]
    public string TourInfo { get; set; } = string.Empty;

    [BindProperty]
    [Display(Name = "Tour Image")]
    public IFormFile? ImageFile { get; set; }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Users/Login");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Users/Login");
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
                CreatedBy = HttpContext.Session.GetString("UserEmail") ?? "Admin",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "tours");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream, cancellationToken);
                }

                tour.ImageFileName = uniqueFileName;
            }

            await _tourService.CreateTourAsync(tour, cancellationToken);

            _logger.LogInformation("Tour created: {TourName}", tour.TourName);
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

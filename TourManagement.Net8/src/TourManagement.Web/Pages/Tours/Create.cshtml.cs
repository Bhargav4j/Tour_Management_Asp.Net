using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

public class CreateTourPageModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<CreateTourPageModel> _logger;
    private readonly IWebHostEnvironment _environment;

    public CreateTourPageModel(ITourService tourService, ILogger<CreateTourPageModel> logger, IWebHostEnvironment environment)
    {
        _tourService = tourService;
        _logger = logger;
        _environment = environment;
    }

    [BindProperty]
    public TourViewModel Input { get; set; } = new();

    [TempData]
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

            // Handle file upload
            if (Input.PictureFile != null && Input.PictureFile.Length > 0)
            {
                // Validate file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(Input.PictureFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ErrorMessage = "Invalid file type. Only JPG, JPEG, PNG, and GIF files are allowed.";
                    return Page();
                }

                if (Input.PictureFile.Length > 5242880) // 5MB
                {
                    ErrorMessage = "File size exceeds maximum limit of 5MB.";
                    return Page();
                }

                // Save file
                fileName = $"{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "tour-pictures");
                Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.PictureFile.CopyToAsync(stream);
                }
            }

            // Map ViewModel to DTO
            var createDto = new TourCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                PictureFileName = fileName
            };

            await _tourService.CreateAsync(createDto);

            _logger.LogInformation("Tour created successfully: {TourName}", Input.TourName);

            TempData["SuccessMessage"] = "Tour added successfully!";
            return RedirectToPage("/Tours/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", Input.TourName);
            ErrorMessage = "An error occurred while adding the tour. Please try again.";
            return Page();
        }
    }
}

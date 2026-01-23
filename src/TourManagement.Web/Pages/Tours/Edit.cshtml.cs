using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;
    private readonly IConfiguration _configuration;

    public EditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<EditModel> logger, IConfiguration configuration)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
        _configuration = configuration;
    }

    public Tour? Tour { get; set; }

    [BindProperty]
    public int TourId { get; set; }

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
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [BindProperty]
    public string Locations { get; set; } = string.Empty;

    [BindProperty]
    public string TourInfo { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

    public string? CurrentPicturePath { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Tour = await _tourService.GetByIdAsync(id);

            if (Tour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
                return NotFound();
            }

            TourId = Tour.Id;
            TourName = Tour.TourName;
            Place = Tour.Place;
            Days = Tour.Days;
            Price = Tour.Price;
            Locations = Tour.Locations;
            TourInfo = Tour.TourInfo;
            CurrentPicturePath = Tour.PicturePath;

            _logger.LogInformation("Edit page loaded for tour ID: {TourId}", id);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for editing, ID: {TourId}", id);
            return RedirectToPage("/Tours/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            Tour = await _tourService.GetByIdAsync(id);
            CurrentPicturePath = Tour?.PicturePath;
            return Page();
        }

        try
        {
            Tour = await _tourService.GetByIdAsync(id);

            if (Tour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
                return NotFound();
            }

            string? picturePath = Tour.PicturePath;

            if (PictureFile != null && PictureFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(PictureFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ErrorMessage = "Invalid file type. Only images are allowed.";
                    CurrentPicturePath = Tour.PicturePath;
                    return Page();
                }

                if (PictureFile.Length > 5 * 1024 * 1024)
                {
                    ErrorMessage = "File size must not exceed 5MB.";
                    CurrentPicturePath = Tour.PicturePath;
                    return Page();
                }

                // Get upload path from configuration (supports persistent volumes or cloud storage)
                var uploadsFolder = _configuration.GetValue<string>("FileStorage:UploadPath");
                if (string.IsNullOrEmpty(uploadsFolder))
                {
                    uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                }
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(Tour.PicturePath))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, Tour.PicturePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                picturePath = uniqueFileName;
            }

            Tour.TourName = TourName;
            Tour.Place = Place;
            Tour.Days = Days;
            Tour.Price = Price;
            Tour.Locations = Locations;
            Tour.TourInfo = TourInfo;
            Tour.PicturePath = picturePath;

            await _tourService.UpdateAsync(id, Tour);

            _logger.LogInformation("Tour {TourId} updated successfully", id);

            TempData["SuccessMessage"] = "Tour updated successfully!";
            return RedirectToPage("/Tours/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour ID: {TourId}", id);
            ErrorMessage = "An error occurred while updating the tour. Please try again.";
            Tour = await _tourService.GetByIdAsync(id);
            CurrentPicturePath = Tour?.PicturePath;
            return Page();
        }
    }
}

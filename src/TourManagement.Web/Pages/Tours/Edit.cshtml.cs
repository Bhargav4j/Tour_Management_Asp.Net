using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

[Authorize]
public class TourEditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<TourEditModel> _logger;

    public TourEditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<TourEditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public int Id { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(200)]
    public string TourName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Place is required")]
    [StringLength(200)]
    public string Place { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    public int Days { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Locations is required")]
    [StringLength(500)]
    public string Locations { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Tour information is required")]
    [StringLength(2000)]
    public string TourInfo { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

    [BindProperty]
    public string? CurrentPicturePath { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var tour = await _tourService.GetTourByIdAsync(id, cancellationToken);

            if (tour == null)
            {
                return NotFound();
            }

            Id = tour.Id;
            TourName = tour.TourName;
            Place = tour.Place;
            Days = tour.Days;
            Price = tour.Price;
            Locations = tour.Locations;
            TourInfo = tour.TourInfo;
            CurrentPicturePath = tour.PicturePath;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit: {TourId}", id);
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var tour = await _tourService.GetTourByIdAsync(Id, cancellationToken);

            if (tour == null)
            {
                return NotFound();
            }

            string? picturePath = CurrentPicturePath;

            if (PictureFile != null && PictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Tour_pics");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{PictureFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(fileStream, cancellationToken);
                }

                if (!string.IsNullOrEmpty(CurrentPicturePath))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, CurrentPicturePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                picturePath = uniqueFileName;
            }

            tour.TourName = TourName;
            tour.Place = Place;
            tour.Days = Days;
            tour.Price = Price;
            tour.Locations = Locations;
            tour.TourInfo = TourInfo;
            tour.PicturePath = picturePath;

            await _tourService.UpdateTourAsync(tour, cancellationToken);

            _logger.LogInformation("Tour updated successfully: {TourId}", Id);

            return RedirectToPage("/Tours/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour: {TourId}", Id);
            ErrorMessage = "An error occurred while updating the tour. Please try again.";
            return Page();
        }
    }
}

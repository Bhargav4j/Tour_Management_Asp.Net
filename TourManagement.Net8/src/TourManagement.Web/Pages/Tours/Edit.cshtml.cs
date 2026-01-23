using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tours;

public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<EditModel> _logger;
    private readonly IWebHostEnvironment _environment;

    public EditModel(
        ITourService tourService,
        ILogger<EditModel> logger,
        IWebHostEnvironment environment)
    {
        _tourService = tourService;
        _logger = logger;
        _environment = environment;
    }

    [BindProperty]
    public TourEditInputModel TourInput { get; set; } = new();

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var tour = await _tourService.GetByIdAsync(id);

            if (tour == null)
            {
                return NotFound();
            }

            TourInput = new TourEditInputModel
            {
                Id = tour.Id,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price,
                Locations = tour.Locations,
                TourInfo = tour.TourInfo,
                CurrentPictureFileName = tour.PictureFileName,
                IsActive = tour.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for editing, ID: {TourId}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var updateDto = new TourUpdateDto
            {
                TourName = TourInput.TourName,
                Place = TourInput.Place,
                Days = TourInput.Days,
                Price = TourInput.Price,
                Locations = TourInput.Locations,
                TourInfo = TourInput.TourInfo,
                PictureFileName = TourInput.CurrentPictureFileName,
                IsActive = TourInput.IsActive
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

                if (!string.IsNullOrEmpty(TourInput.CurrentPictureFileName))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, TourInput.CurrentPictureFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                updateDto.PictureFileName = uniqueFileName;
            }

            await _tourService.UpdateAsync(TourInput.Id, updateDto);

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID: {TourId}", TourInput.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour.");
            return Page();
        }
    }
}

public class TourEditInputModel
{
    public int Id { get; set; }

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

    public string? CurrentPictureFileName { get; set; }

    public bool IsActive { get; set; }
}

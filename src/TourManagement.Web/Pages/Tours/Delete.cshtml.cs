using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(
        ITourService tourService,
        IWebHostEnvironment environment,
        ILogger<DeleteModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    public TourDeleteViewModel? Tour { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var tourDto = await _tourService.GetByIdAsync(id);

            if (tourDto == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
                TempData["ErrorMessage"] = $"Tour with ID {id} not found.";
                return RedirectToPage("./Index");
            }

            // Manual mapping from TourDto to TourDeleteViewModel
            Tour = new TourDeleteViewModel
            {
                Id = tourDto.Id,
                TourName = tourDto.TourName,
                Place = tourDto.Place,
                Days = tourDto.Days,
                Price = tourDto.Price,
                Locations = tourDto.Locations,
                TourInfo = tourDto.TourInfo,
                PictureFileName = tourDto.PictureFileName,
                CreatedDate = tourDto.CreatedDate,
                ModifiedDate = tourDto.ModifiedDate,
                IsActive = tourDto.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour with ID: {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the tour.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var tourDto = await _tourService.GetByIdAsync(id);

            if (tourDto == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found during deletion", id);
                TempData["ErrorMessage"] = $"Tour with ID {id} not found.";
                return RedirectToPage("./Index");
            }

            // Store the picture filename and tour name before deletion
            var pictureFileName = tourDto.PictureFileName;
            var tourName = tourDto.TourName;

            // Delete the tour from the database
            await _tourService.DeleteAsync(id);

            // Delete the associated image file if it exists
            if (!string.IsNullOrEmpty(pictureFileName))
            {
                DeleteImageFile(pictureFileName);
            }

            _logger.LogInformation("Tour deleted successfully with ID: {TourId}", id);
            TempData["SuccessMessage"] = $"Tour '{tourName}' has been deleted successfully.";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID: {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the tour. Please try again.";
            return RedirectToPage("./Index");
        }
    }

    private void DeleteImageFile(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_environment.WebRootPath, "images", "tours", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                _logger.LogInformation("Image deleted successfully: {FileName}", fileName);
            }
            else
            {
                _logger.LogWarning("Image file not found: {FileName}", fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image file: {FileName}", fileName);
            // Don't throw - we don't want to fail the deletion if image deletion fails
            // The database record is already deleted at this point
        }
    }

    public class TourDeleteViewModel
    {
        public int Id { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public int Days { get; set; }
        public decimal Price { get; set; }
        public string Locations { get; set; } = string.Empty;
        public string TourInfo { get; set; } = string.Empty;
        public string? PictureFileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}

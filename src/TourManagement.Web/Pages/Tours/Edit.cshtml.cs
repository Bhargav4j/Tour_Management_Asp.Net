using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        ITourService tourService,
        IWebHostEnvironment environment,
        ILogger<EditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public TourEditInputModel Input { get; set; } = new TourEditInputModel();

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

            // Manual mapping from TourDto to TourEditInputModel
            Input = new TourEditInputModel
            {
                Id = tourDto.Id,
                TourName = tourDto.TourName,
                Place = tourDto.Place,
                Days = tourDto.Days,
                Price = tourDto.Price,
                Locations = tourDto.Locations,
                TourInfo = tourDto.TourInfo,
                CurrentPictureFileName = tourDto.PictureFileName,
                IsActive = tourDto.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit with ID: {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the tour.";
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
            var existingTour = await _tourService.GetByIdAsync(Input.Id);

            if (existingTour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found during update", Input.Id);
                TempData["ErrorMessage"] = $"Tour with ID {Input.Id} not found.";
                return RedirectToPage("./Index");
            }

            string? pictureFileName = Input.CurrentPictureFileName;

            // Handle image file operations
            if (Input.DeleteExistingImage && !string.IsNullOrEmpty(Input.CurrentPictureFileName))
            {
                // Delete existing image
                DeleteImageFile(Input.CurrentPictureFileName);
                pictureFileName = null;
            }

            if (Input.ImageFile != null)
            {
                // Delete old image if exists and we're uploading a new one
                if (!string.IsNullOrEmpty(Input.CurrentPictureFileName))
                {
                    DeleteImageFile(Input.CurrentPictureFileName);
                }

                // Save new image
                pictureFileName = await SaveImageFileAsync(Input.ImageFile);
            }

            // Manual mapping from InputModel to UpdateDto
            var updateDto = new TourUpdateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                PictureFileName = pictureFileName
            };

            await _tourService.UpdateAsync(Input.Id, updateDto);

            _logger.LogInformation("Tour updated successfully with ID: {TourId}", Input.Id);
            TempData["SuccessMessage"] = $"Tour '{Input.TourName}' updated successfully.";

            return RedirectToPage("./Details", new { id = Input.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID: {TourId}", Input.Id);
            TempData["ErrorMessage"] = "An error occurred while updating the tour. Please try again.";
            return Page();
        }
    }

    private async Task<string> SaveImageFileAsync(IFormFile imageFile)
    {
        try
        {
            // Validate file size (5MB limit)
            if (imageFile.Length > 5 * 1024 * 1024)
            {
                throw new InvalidOperationException("File size exceeds 5MB limit.");
            }

            // Validate file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Invalid file format. Only JPG, PNG, and GIF are allowed.");
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "tours");

            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            _logger.LogInformation("Image saved successfully: {FileName}", fileName);
            return fileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving image file");
            throw;
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image file: {FileName}", fileName);
            // Don't throw - we don't want to fail the update if image deletion fails
        }
    }

    public class TourEditInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tour name is required")]
        [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
        public string TourName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Place is required")]
        [StringLength(100, ErrorMessage = "Place cannot exceed 100 characters")]
        public string Place { get; set; } = string.Empty;

        [Required(ErrorMessage = "Days is required")]
        [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
        public int Days { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Locations is required")]
        [StringLength(500, ErrorMessage = "Locations cannot exceed 500 characters")]
        public string Locations { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tour information is required")]
        [StringLength(2000, ErrorMessage = "Tour information cannot exceed 2000 characters")]
        public string TourInfo { get; set; } = string.Empty;

        public string? CurrentPictureFileName { get; set; }

        public IFormFile? ImageFile { get; set; }

        public bool DeleteExistingImage { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

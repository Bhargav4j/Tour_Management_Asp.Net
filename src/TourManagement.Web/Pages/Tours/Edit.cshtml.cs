using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    public EditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<EditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public Tour Tour { get; set; } = new Tour();

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            Tour = await _tourService.GetTourByIdAsync(id.Value) ?? new Tour();

            if (Tour.Id == 0)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit with ID {TourId}", id);
            return NotFound();
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
            if (PictureFile != null)
            {
                Tour.PictureFileName = await SaveFileAsync(PictureFile);
            }

            Tour.ModifiedBy = "Admin";
            Tour.ModifiedDate = DateTime.UtcNow;

            await _tourService.UpdateTourAsync(Tour);

            _logger.LogInformation("Tour updated successfully: {TourName}", Tour.TourName);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", Tour.Id);
            Message = "Error updating tour. Please try again.";
            IsSuccess = false;
            return Page();
        }
    }

    private async Task<string> SaveFileAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "Tour_pics");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return uniqueFileName;
    }
}

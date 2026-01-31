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

    [BindProperty]
    public Tour Tour { get; set; } = null!;

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var tour = await _tourService.GetTourByIdAsync(id.Value, cancellationToken);

            if (tour == null)
            {
                return NotFound();
            }

            Tour = tour;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for editing");
            ErrorMessage = "An error occurred while loading the tour.";
            return Page();
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
            if (PictureFile != null && PictureFile.Length > 0)
            {
                var uploadPath = _configuration.GetValue<string>("UPLOAD_PATH") ?? "/app/uploads";
                var uploadsFolder = Path.Combine(uploadPath, "images", "tours");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(PictureFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(fileStream, cancellationToken);
                }

                if (!string.IsNullOrEmpty(Tour.Picture))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, Tour.Picture);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                Tour.Picture = fileName;
            }

            Tour.ModifiedBy = User.Identity?.Name ?? "System";

            await _tourService.UpdateTourAsync(Tour, cancellationToken);

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour");
            ErrorMessage = "An error occurred while updating the tour.";
            return Page();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;

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
    public TourViewModel Tour { get; set; } = new TourViewModel();

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var tourDto = await _tourService.GetByIdAsync(id.Value);

            if (tourDto == null)
            {
                return NotFound();
            }

            Tour = new TourViewModel
            {
                Id = tourDto.Id,
                TourName = tourDto.TourName,
                Place = tourDto.Place,
                Days = tourDto.Days,
                Price = tourDto.Price,
                Locations = tourDto.Locations,
                TourInfo = tourDto.TourInfo,
                CurrentPicturePath = tourDto.PicturePath
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit with ID {TourId}", id);
            ModelState.AddModelError(string.Empty, "Error loading tour. Please try again.");
            return Page();
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
            string? picturePath = Tour.CurrentPicturePath;

            if (PictureFile != null && PictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "tours");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + PictureFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(fileStream);
                }

                picturePath = uniqueFileName;
            }

            var dto = new TourUpdateDto
            {
                TourName = Tour.TourName,
                Place = Tour.Place,
                Days = Tour.Days,
                Price = Tour.Price,
                Locations = Tour.Locations,
                TourInfo = Tour.TourInfo,
                PicturePath = picturePath
            };

            await _tourService.UpdateAsync(Tour.Id, dto);

            TempData["SuccessMessage"] = "Tour updated successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", Tour.Id);
            ModelState.AddModelError(string.Empty, "Error updating tour. Please try again.");
            return Page();
        }
    }

    public class TourViewModel
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
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(500)]
        public string Locations { get; set; } = string.Empty;

        [StringLength(2000)]
        public string TourInfo { get; set; } = string.Empty;

        public string? CurrentPicturePath { get; set; }
    }
}

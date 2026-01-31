using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

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
    public TourEditModel Tour { get; set; } = new TourEditModel();

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

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

            Tour = new TourEditModel
            {
                Id = tourDto.Id,
                TourName = tourDto.TourName,
                Place = tourDto.Place,
                Days = tourDto.Days,
                Price = tourDto.Price,
                Locations = tourDto.Locations,
                TourInfo = tourDto.TourInfo,
                PictureFileName = tourDto.PictureFileName,
                IsActive = tourDto.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit, ID: {TourId}", id);
            return RedirectToPage("Index");
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
            string? fileName = Tour.PictureFileName;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(Tour.PictureFileName))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, Tour.PictureFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }

            var updateDto = new TourUpdateDto
            {
                TourName = Tour.TourName,
                Place = Tour.Place,
                Days = Tour.Days,
                Price = Tour.Price,
                Locations = Tour.Locations,
                TourInfo = Tour.TourInfo,
                PictureFileName = fileName,
                IsActive = Tour.IsActive
            };

            await _tourService.UpdateAsync(Tour.Id, updateDto, "System");
            TempData["SuccessMessage"] = "Tour updated successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour");
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour. Please try again.");
            return Page();
        }
    }

    public class TourEditModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Tour Name")]
        public string TourName { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Place { get; set; } = string.Empty;

        [Required]
        [Range(1, 365)]
        public int Days { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(500)]
        public string Locations { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        [Display(Name = "Tour Information")]
        public string TourInfo { get; set; } = string.Empty;

        public string? PictureFileName { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class DetailsModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(ITourService tourService, ILogger<DetailsModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public TourDetailViewModel? Tour { get; set; }

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

            // Manual mapping from TourDto to TourDetailViewModel
            Tour = new TourDetailViewModel
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
            TempData["ErrorMessage"] = "An error occurred while loading the tour details.";
            return RedirectToPage("./Index");
        }
    }

    public class TourDetailViewModel
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

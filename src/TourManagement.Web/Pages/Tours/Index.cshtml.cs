using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public IEnumerable<TourViewModel> Tours { get; set; } = new List<TourViewModel>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            IEnumerable<TourDto> tourDtos;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                tourDtos = await _tourService.SearchAsync(SearchTerm);
            }
            else
            {
                tourDtos = await _tourService.GetAllAsync();
            }

            // Manual mapping from TourDto to TourViewModel
            Tours = tourDtos.Select(dto => new TourViewModel
            {
                Id = dto.Id,
                TourName = dto.TourName,
                Place = dto.Place,
                Days = dto.Days,
                Price = dto.Price,
                Locations = dto.Locations,
                TourInfo = dto.TourInfo,
                PictureFileName = dto.PictureFileName,
                CreatedDate = dto.CreatedDate,
                ModifiedDate = dto.ModifiedDate,
                IsActive = dto.IsActive
            }).ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tours");
            TempData["ErrorMessage"] = "An error occurred while loading tours.";
            Tours = new List<TourViewModel>();
            return Page();
        }
    }

    public class TourViewModel
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

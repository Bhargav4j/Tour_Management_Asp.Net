using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;
using TourManagement.Domain.Entities;

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

    public List<TourViewModel> Tours { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            var tours = await _tourService.GetAllAsync();
            Tours = tours.Select(t => MapToViewModel(t)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours");
            Tours = new List<TourViewModel>();
        }
    }

    private static TourViewModel MapToViewModel(Tour tour)
    {
        return new TourViewModel
        {
            Id = tour.Id,
            Name = tour.Name,
            Place = tour.Place,
            Days = tour.Days,
            Price = tour.Price,
            Locations = tour.Locations,
            TourInfo = tour.TourInfo,
            PictureFileName = tour.PictureFileName
        };
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Web.Pages.Admin;

public class DashboardModel : PageModel
{
    private readonly ITourRepository _tourRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<DashboardModel> _logger;
    private readonly IWebHostEnvironment _environment;

    public DashboardModel(
        ITourRepository tourRepository,
        IUserRepository userRepository,
        IBookingRepository bookingRepository,
        ILogger<DashboardModel> logger,
        IWebHostEnvironment environment)
    {
        _tourRepository = tourRepository;
        _userRepository = userRepository;
        _bookingRepository = bookingRepository;
        _logger = logger;
        _environment = environment;
    }

    // Statistics Properties
    public int TotalTours { get; set; }
    public int ActiveTours { get; set; }
    public int TotalUsers { get; set; }
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int TodaysBookings { get; set; }
    public int ThisWeekBookings { get; set; }
    public decimal TotalRevenue { get; set; }

    // Admin Info Properties
    public string AdminEmail { get; set; } = string.Empty;
    public string LoginTime { get; set; } = string.Empty;
    public string SessionDuration { get; set; } = string.Empty;
    public string CurrentDateTime { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if admin is logged in
        if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
        {
            TempData["ErrorMessage"] = "Please login to access the admin dashboard.";
            return RedirectToPage("/Admin/Login");
        }

        // Get admin email from session
        AdminEmail = HttpContext.Session.GetString("AdminEmail") ?? "Admin";

        // Get login time and calculate session duration
        var loginTimeStr = HttpContext.Session.GetString("AdminLoginTime");
        if (!string.IsNullOrEmpty(loginTimeStr) && DateTime.TryParse(loginTimeStr, out var loginDateTime))
        {
            LoginTime = loginDateTime.ToLocalTime().ToString("MMM dd, yyyy hh:mm tt");
            var duration = DateTime.UtcNow - loginDateTime;
            SessionDuration = $"{duration.Hours}h {duration.Minutes}m";
        }
        else
        {
            LoginTime = "Unknown";
            SessionDuration = "N/A";
        }

        // Current date/time
        CurrentDateTime = DateTime.Now.ToString("MMM dd, yyyy hh:mm tt");

        // Environment
        Environment = _environment.EnvironmentName;

        try
        {
            // Get all statistics
            await LoadStatisticsAsync();

            _logger.LogInformation("Admin dashboard loaded successfully for {Email} at {Time}",
                AdminEmail, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard statistics for admin {Email}", AdminEmail);
            TempData["ErrorMessage"] = "Error loading dashboard statistics. Please try again.";
        }

        return Page();
    }

    public IActionResult OnPostLogout()
    {
        // Log the logout
        var adminEmail = HttpContext.Session.GetString("AdminEmail") ?? "Unknown";
        _logger.LogInformation("Admin user {Email} logged out at {Time}",
            adminEmail, DateTime.UtcNow);

        // Clear session
        HttpContext.Session.Remove("AdminLoggedIn");
        HttpContext.Session.Remove("AdminEmail");
        HttpContext.Session.Remove("AdminLoginTime");

        // Clear remember me cookie
        if (Request.Cookies["AdminRememberMe"] != null)
        {
            Response.Cookies.Delete("AdminRememberMe");
        }

        TempData["LogoutMessage"] = "You have been logged out successfully.";
        return RedirectToPage("/Admin/Login");
    }

    private async Task LoadStatisticsAsync()
    {
        // Get all tours
        var allTours = await _tourRepository.GetAllAsync();
        TotalTours = allTours.Count();
        ActiveTours = allTours.Count(t => t.IsActive);

        // Get all users
        var allUsers = await _userRepository.GetAllAsync();
        TotalUsers = allUsers.Count();

        // Get all bookings
        var allBookings = await _bookingRepository.GetAllAsync();
        TotalBookings = allBookings.Count();

        // Calculate booking statistics
        PendingBookings = allBookings.Count(b => b.Status == "Pending");
        ConfirmedBookings = allBookings.Count(b => b.Status == "Confirmed");

        // Today's bookings
        var today = DateTime.Today;
        TodaysBookings = allBookings.Count(b => b.BookingDate.Date == today);

        // This week's bookings
        var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        ThisWeekBookings = allBookings.Count(b => b.BookingDate >= weekStart);

        // Calculate total revenue from confirmed bookings
        var confirmedBookingsList = allBookings.Where(b => b.Status == "Confirmed").ToList();
        TotalRevenue = 0;

        foreach (var booking in confirmedBookingsList)
        {
            // Get tour for this booking to get the price
            if (booking.TourId.HasValue)
            {
                var tour = await _tourRepository.GetByIdAsync(booking.TourId.Value);
                if (tour != null)
                {
                    // Assuming 1 person per booking since NumberOfPeople is not in the entity
                    TotalRevenue += tour.Price;
                }
            }
        }
    }
}

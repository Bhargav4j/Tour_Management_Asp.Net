using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Booking entity
/// </summary>
public class BookingRepository : IBookingRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<BookingRepository> _logger;

    public BookingRepository(
        TourManagementDbContext context,
        ILogger<BookingRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bookings
                .Where(b => b.IsActive)
                .Include(b => b.Tour)
                .Include(b => b.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings from database");
            throw;
        }
    }

    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bookings
                .Where(b => b.BookingId == id && b.IsActive)
                .Include(b => b.Tour)
                .Include(b => b.User)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking {BookingId} from database", id);
            throw;
        }
    }

    public async Task<Booking> AddAsync(Booking entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Bookings.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding booking to database");
            throw;
        }
    }

    public async Task UpdateAsync(Booking entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Bookings.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking {BookingId} in database", entity.BookingId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await GetByIdAsync(id, cancellationToken);
            if (booking != null)
            {
                booking.IsActive = false;
                booking.ModifiedDate = DateTime.UtcNow;
                await UpdateAsync(booking, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking {BookingId} from database", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bookings
                .AnyAsync(b => b.BookingId == id && b.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if booking {BookingId} exists in database", id);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bookings
                .Where(b => b.Email == email && b.IsActive)
                .Include(b => b.Tour)
                .Include(b => b.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user {Email} from database", email);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bookings
                .Where(b => b.IsActive &&
                    (b.Email.Contains(searchTerm) ||
                     b.FirstName.Contains(searchTerm) ||
                     b.TourName.Contains(searchTerm)))
                .Include(b => b.Tour)
                .Include(b => b.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching bookings in database");
            throw;
        }
    }
}

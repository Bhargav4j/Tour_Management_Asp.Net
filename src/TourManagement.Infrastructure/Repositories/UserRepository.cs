using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for UserInfo entity
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(TourManagementDbContext context, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users from database");
            throw;
        }
    }

    public async Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email: {Email} from database", email);
            throw;
        }
    }

    public async Task<UserInfo> AddAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user to database");
            throw;
        }
    }

    public async Task UpdateAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with email: {Email} in database", user.Email);
            throw;
        }
    }

    public async Task DeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users.FindAsync(new object[] { email }, cancellationToken);
            if (user != null)
            {
                user.IsActive = false;
                user.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email: {Email} from database", email);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email && u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user with email: {Email} exists in database", email);
            throw;
        }
    }

    public async Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive &&
                    (u.FirstName.Contains(searchTerm) ||
                     u.LastName.Contains(searchTerm) ||
                     u.Email.Contains(searchTerm)))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm} in database", searchTerm);
            throw;
        }
    }

    public async Task<UserInfo?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user with email: {Email} in database", email);
            throw;
        }
    }
}

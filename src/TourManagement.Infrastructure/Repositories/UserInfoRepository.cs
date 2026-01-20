using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for UserInfo entity
/// </summary>
public class UserInfoRepository : IUserInfoRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<UserInfoRepository> _logger;

    public UserInfoRepository(TourManagementDbContext context, ILogger<UserInfoRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInfos
                .AsNoTracking()
                .Where(u => u.IsActive)
                .OrderBy(u => u.Email)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users from database");
            throw;
        }
    }

    public async Task<UserInfo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId} from database", id);
            throw;
        }
    }

    public async Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email} from database", email);
            throw;
        }
    }

    public async Task<UserInfo> AddAsync(UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.UserInfos.AddAsync(userInfo, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user to database");
            throw;
        }
    }

    public async Task UpdateAsync(UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.UserInfos.Update(userInfo);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId} in database", userInfo.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.UserInfos.FindAsync(new object[] { id }, cancellationToken);
            if (user != null)
            {
                user.IsActive = false;
                user.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId} from database", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInfos.AnyAsync(u => u.Id == id && u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user with ID {UserId} exists in database", id);
            throw;
        }
    }

    public async Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync(cancellationToken);
            }

            return await _context.UserInfos
                .AsNoTracking()
                .Where(u => u.IsActive &&
                    (u.Email.Contains(searchTerm) ||
                     u.FirstName.Contains(searchTerm) ||
                     u.LastName.Contains(searchTerm) ||
                     u.City.Contains(searchTerm)))
                .OrderBy(u => u.Email)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term {SearchTerm} in database", searchTerm);
            throw;
        }
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.UserInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive, cancellationToken);

            if (user == null)
            {
                return false;
            }

            return user.Password == password;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating credentials for email {Email} in database", email);
            throw;
        }
    }
}

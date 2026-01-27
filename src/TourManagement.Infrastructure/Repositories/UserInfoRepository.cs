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

    public UserInfoRepository(
        TourManagementDbContext context,
        ILogger<UserInfoRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInfos
                .Where(u => u.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users from database");
            throw;
        }
    }

    public async Task<UserInfo?> GetByIdAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInfos
                .Where(u => u.Email == email && u.IsActive)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {Email} from database", email);
            throw;
        }
    }

    public async Task<UserInfo> AddAsync(UserInfo entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.UserInfos.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user to database");
            throw;
        }
    }

    public async Task UpdateAsync(UserInfo entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.UserInfos.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {Email} in database", entity.Email);
            throw;
        }
    }

    public async Task DeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await GetByIdAsync(email, cancellationToken);
            if (user != null)
            {
                user.IsActive = false;
                user.ModifiedDate = DateTime.UtcNow;
                await UpdateAsync(user, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Email} from database", email);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInfos
                .AnyAsync(u => u.Email == email && u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {Email} exists in database", email);
            throw;
        }
    }

    public async Task<UserInfo?> GetByEmailAndPasswordAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInfos
                .Where(u => u.Email == email && u.Password == password && u.IsActive)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user credentials for {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInfos
                .Where(u => u.IsActive &&
                    (u.Email.Contains(searchTerm) ||
                     u.FirstName.Contains(searchTerm) ||
                     u.LastName.Contains(searchTerm)))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users in database");
            throw;
        }
    }
}

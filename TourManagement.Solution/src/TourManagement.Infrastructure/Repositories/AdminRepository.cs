using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Admin entity
/// </summary>
public class AdminRepository : IAdminRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<AdminRepository> _logger;

    public AdminRepository(
        TourManagementDbContext context,
        ILogger<AdminRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Username == username, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin with username {Username} from database", username);
            throw;
        }
    }

    public async Task<Admin?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin with ID {AdminId} from database", id);
            throw;
        }
    }

    public async Task<Admin> AddAsync(Admin admin, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync(cancellationToken);
            return admin;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding admin to database");
            throw;
        }
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var admin = await GetByUsernameAsync(username, cancellationToken);
            if (admin == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating admin credentials for username {Username}", username);
            throw;
        }
    }
}

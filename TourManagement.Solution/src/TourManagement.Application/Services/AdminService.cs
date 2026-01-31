using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Contracts.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Admin business operations
/// </summary>
public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IAdminRepository adminRepository,
        IMapper mapper,
        ILogger<AdminService> logger)
    {
        _adminRepository = adminRepository ?? throw new ArgumentNullException(nameof(adminRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> ValidateLoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Admin login attempt: {Username}", username);

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var isValid = await _adminRepository.ValidateCredentialsAsync(username, password, cancellationToken);

            if (isValid)
            {
                _logger.LogInformation("Admin logged in successfully: {Username}", username);
            }
            else
            {
                _logger.LogWarning("Admin login failed: {Username}", username);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin login: {Username}", username);
            throw;
        }
    }

    public async Task<AdminDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving admin by username: {Username}", username);
            var admin = await _adminRepository.GetByUsernameAsync(username, cancellationToken);

            if (admin == null)
            {
                _logger.LogWarning("Admin with username {Username} not found", username);
                return null;
            }

            return _mapper.Map<AdminDto>(admin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin by username: {Username}", username);
            throw;
        }
    }
}

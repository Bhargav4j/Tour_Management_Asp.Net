using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repository, IMapper mapper, ILogger<UserService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByEmailAsync(email, cancellationToken);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default)
    {
        var user = _mapper.Map<User>(dto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var createdUser = await _repository.AddAsync(user, cancellationToken);
        return _mapper.Map<UserDto>(createdUser);
    }

    public async Task UpdateAsync(int id, UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _repository.GetByIdAsync(id, cancellationToken);
        if (existingUser == null) throw new InvalidOperationException($"User with ID {id} not found");
        _mapper.Map(dto, existingUser);
        existingUser.ModifiedDate = DateTime.UtcNow;
        await _repository.UpdateAsync(existingUser, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<UserDto?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByEmailAsync(email, cancellationToken);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }
        return _mapper.Map<UserDto>(user);
    }
}

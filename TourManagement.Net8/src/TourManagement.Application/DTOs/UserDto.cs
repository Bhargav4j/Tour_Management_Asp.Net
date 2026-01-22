namespace TourManagement.Application.DTOs;

/// <summary>
/// Data transfer object for User
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating a new user
/// </summary>
public class UserCreateDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}

/// <summary>
/// DTO for updating an existing user
/// </summary>
public class UserUpdateDto
{
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for user login
/// </summary>
public class UserLoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

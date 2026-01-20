using Microsoft.AspNetCore.Http;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Interface for file storage operations
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves a file and returns the file name
    /// </summary>
    Task<string> SaveFileAsync(IFormFile file, string folder);

    /// <summary>
    /// Deletes a file
    /// </summary>
    Task DeleteFileAsync(string fileName, string folder);

    /// <summary>
    /// Gets the full path or URL to a file
    /// </summary>
    string GetFileUrl(string fileName, string folder);
}

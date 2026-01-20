using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Infrastructure.Services;

/// <summary>
/// Local file storage implementation
/// WARNING: This stores files on local filesystem and is not suitable for containerized environments
/// Consider using cloud storage (Azure Blob, AWS S3, MinIO) for production containers
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IWebHostEnvironment environment, ILogger<LocalFileStorageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folder)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, folder);
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        _logger.LogInformation("File saved locally: {FileName} to {Folder}", uniqueFileName, folder);
        return uniqueFileName;
    }

    public Task DeleteFileAsync(string fileName, string folder)
    {
        var filePath = Path.Combine(_environment.WebRootPath, folder, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _logger.LogInformation("File deleted: {FileName}", fileName);
        }

        return Task.CompletedTask;
    }

    public string GetFileUrl(string fileName, string folder)
    {
        return $"/{folder}/{fileName}";
    }
}

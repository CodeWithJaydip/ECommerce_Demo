using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace ECommerce.Infrastructure.Services;

/// <summary>
/// Service for handling file uploads
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Upload an image file and return the relative path
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="folder">The folder name (e.g., "categories", "products")</param>
    /// <returns>The relative path to the uploaded file</returns>
    Task<string> UploadImageAsync(IFormFile file, string folder);

    /// <summary>
    /// Delete an image file
    /// </summary>
    /// <param name="imagePath">The relative path to the image</param>
    Task DeleteImageAsync(string? imagePath);
}

public class FileUploadService : IFileUploadService
{
    private readonly IHostEnvironment _environment;
    private const string UploadsFolder = "wwwroot/uploads";
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public FileUploadService(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> UploadImageAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty or null");
        }

        // Validate file size
        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException($"File size exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)}MB");
        }

        // Validate file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
        {
            throw new ArgumentException($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", _allowedExtensions)}");
        }

        // Create unique filename
        var fileName = $"{Guid.NewGuid()}{extension}";
        var contentRootPath = _environment.ContentRootPath;
        var wwwrootPath = Path.Combine(contentRootPath, "wwwroot");
        var folderPath = Path.Combine(wwwrootPath, "uploads", folder);
        
        // Create directory if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var filePath = Path.Combine(folderPath, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return relative path from wwwroot (e.g., "uploads/categories/guid.jpg")
        return Path.Combine("uploads", folder, fileName).Replace("\\", "/");
    }

    public async Task DeleteImageAsync(string? imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            return;
        }

        var contentRootPath = _environment.ContentRootPath;
        var fullPath = Path.Combine(contentRootPath, "wwwroot", imagePath.Replace("/", "\\"));
        
        if (File.Exists(fullPath))
        {
            await Task.Run(() => File.Delete(fullPath));
        }
    }
}

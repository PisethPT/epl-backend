using epl_backend.Services.Interfaces;
namespace epl_backend.Services.Implementations;

public class FileStorageService : IFileStorageService
{
    private readonly string[] _allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg", ".webp" };
    private readonly long _maxFileBytes = 2 * 1024 * 1024; // 2 MB
    private readonly string _crestFolder = "upload/crests";
    private readonly string _wwwroot = "wwwroot";
    private readonly IWebHostEnvironment _env;

    public FileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveCrestAsync(IFormFile file, CancellationToken ct = default)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("File is empty", nameof(file));
        if (file.Length > _maxFileBytes) throw new ArgumentException("File exceeds maximum allowed size (2MB).");
        var ext = Path.GetExtension(file.FileName ?? string.Empty);
        if (string.IsNullOrEmpty(ext) || Array.IndexOf(_allowedExtensions, ext.ToLowerInvariant()) < 0)
            throw new ArgumentException("Invalid file type. Allowed: png, jpg, jpeg, svg.");

        var uploadsRoot = Path.Combine(_env.WebRootPath ?? _wwwroot, _crestFolder);
        if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var relativePath = Path.Combine(_crestFolder, fileName).Replace("\\", "/"); // store as relative URL path
        var fullPath = Path.Combine(uploadsRoot, fileName);

        await using var stream = new FileStream(fullPath, FileMode.CreateNew);
        await file.CopyToAsync(stream, ct);

        return relativePath; // e.g. "uploads/crests/1234abcd.png"
    }

    public Task<bool> DeleteCrestAsync(string relativePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return Task.FromResult(false);

        var fullPath = Path.Combine(_env.WebRootPath ?? _wwwroot, relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public async Task<string> SavePhotoAsync(IFormFile file, string folder, CancellationToken ct = default)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("File is empty", nameof(file));
        if (file.Length > _maxFileBytes) throw new ArgumentException("File exceeds maximum allowed size (2MB).");
        var ext = Path.GetExtension(file.FileName ?? string.Empty);
        if (string.IsNullOrEmpty(ext) || Array.IndexOf(_allowedExtensions, ext.ToLowerInvariant()) < 0)
            throw new ArgumentException("Invalid file type. Allowed: png, jpg, jpeg, svg.");

        var uploadsRoot = Path.Combine(_env.WebRootPath ?? _wwwroot, folder);
        if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var relativePath = Path.Combine(folder, fileName).Replace("\\", "/");
        var fullPath = Path.Combine(uploadsRoot, fileName);

        await using var stream = new FileStream(fullPath, FileMode.CreateNew);
        await file.CopyToAsync(stream, ct);

        return fileName; // relativePath
    }

    public async Task<bool> DeleteFileAsync(string relativePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return await Task.FromResult(false);

        var fullPath = Path.Combine(_env.WebRootPath ?? _wwwroot, relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return await Task.FromResult(true);
        }

        return await Task.FromResult(false);
    }
}


namespace epl_backend.Services.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveCrestAsync(IFormFile file, CancellationToken ct = default);
    Task<bool> DeleteCrestAsync(string relativePath, CancellationToken ct = default);

    Task<string> SavePhotoAsync(IFormFile file, string folder, CancellationToken ct = default);
    Task<bool> DeleteFileAsync(string relativePath, CancellationToken ct = default);
}

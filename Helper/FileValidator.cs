namespace PremierLeague_Backend.Helper;

public static class FileValidator
{
    /// <summary>
    /// Validate an uploaded file.
    /// </summary>
    /// <param name="file">IFormFile to validate (can be null).</param>
    /// <param name="allowedExtensions">Allowed extensions including the leading dot (e.g. ".png"). If null, defaults to png/jpg/jpeg/svg.</param>
    /// <param name="maxBytes">Maximum allowed file size in bytes. If null, defaults to 2 * 1024 * 1024 (2 MB).</param>
    /// <returns>FileValidationResult describing whether the file is valid and why not.</returns>
    public static FileValidationResult Validate(IFormFile? file, IEnumerable<string>? allowedExtensions = null, long? maxBytes = null)
    {
        if (file == null || file.Length == 0)
            return FileValidationResult.Success();

        var allowed = allowedExtensions != null
            ? new HashSet<string>(allowedExtensions, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".svg", ".webp" };

        long max = maxBytes ?? 2 * 1024 * 1024; // 2 MB

        var ext = Path.GetExtension(file.FileName ?? string.Empty);
        if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext))
        {
            var allowedList = string.Join(", ", allowed.Select(e => e.TrimStart('.').ToUpperInvariant()));
            return FileValidationResult.Fail($"Invalid file type. Allowed: {allowedList}.");
        }

        if (file.Length > max)
        {
            return FileValidationResult.Fail($"File is too large. Max allowed is {max:n0} bytes ({max / 1024 / 1024.0:0.##} MB).");
        }

        return FileValidationResult.Success();
    }
}

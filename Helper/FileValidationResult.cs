namespace PremierLeague_Backend.Helper;

public class FileValidationResult
{
    public bool IsValid { get; }
    public string? ErrorMessage { get; }

    private FileValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static FileValidationResult Success() => new(true);
    public static FileValidationResult Fail(string message) => new(false, message);
}


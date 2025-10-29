namespace VisionaryCoder.Framework;

/// Non-generic result wrapper for operations that don't return a value.
public sealed class ServiceResult
{
    private ServiceResult(bool isSuccess, string? errorMessage, Exception? exception)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    /// Creates a successful result.
    public static ServiceResult Success()
    {
        return new(true, null, null);
    }

    public static ServiceResult Failure(string errorMessage)
    {
        return new(false, errorMessage, null);
    }

    public static ServiceResult Failure(Exception exception)
    {
        return new(false, exception.Message, exception);
    }

    public static ServiceResult Failure(string errorMessage, Exception exception)
    {
        return new(false, errorMessage, exception);
    }

    public void Match(Action onSuccess, Action<string, Exception?> onFailure)
    {
        if (IsSuccess)
        {
            onSuccess();
        }
        else
        {
            onFailure(ErrorMessage ?? "Unknown error", Exception);
        }
    }
    
    /// Gets a value indicating whether the operation was successful.
    public bool IsSuccess { get; }

    /// Gets a value indicating whether the operation failed.
    public bool IsFailure => !IsSuccess;
    
    /// Gets the error message if the operation failed.
    public string? ErrorMessage { get; }

    /// Gets the exception if the operation failed with an exception.
    public Exception? Exception { get; }

}


namespace VisionaryCoder.Framework;

/// <summary>
/// Result wrapper for framework operations that provides consistent success/failure handling.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public sealed class ServiceResult<T>
{
    private ServiceResult(bool isSuccess, T? value, string? errorMessage, Exception? exception)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        Exception = exception;
    }
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }
    /// Gets a value indicating whether the operation failed.
    public bool IsFailure => !IsSuccess;
    /// Gets the result value if the operation was successful.
    public T? Value { get; }
    /// Gets the error message if the operation failed.
    public string? ErrorMessage { get; }
    /// Gets the exception if the operation failed with an exception.
    public Exception? Exception { get; }
    /// Creates a successful result with a value.
    /// <param name="value">The result value.</param>
    /// <returns>A successful result.</returns>
    public static ServiceResult<T> Success(T value) => new(true, value, null, null);
    /// Creates a failed result with an error message.
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed result.</returns>
    public static ServiceResult<T> Failure(string errorMessage) => new(false, default, errorMessage, null);
    /// Creates a failed result with an exception.
    /// <param name="exception">The exception that caused the failure.</param>
    public static ServiceResult<T> Failure(Exception exception) => new(false, default, exception.Message, exception);
    /// Creates a failed result with an error message and exception.
    public static ServiceResult<T> Failure(string errorMessage, Exception exception) => new(false, default, errorMessage, exception);
    /// Matches the result and executes the appropriate action.
    /// <param name="onSuccess">Action to execute if the result is successful.</param>
    /// <param name="onFailure">Action to execute if the result is a failure.</param>
    public void Match(Action<T> onSuccess, Action<string, Exception?> onFailure)
    {
        if (IsSuccess && Value is not null)
        {
            onSuccess(Value);
        }
        else
        { 
            onFailure(ErrorMessage ?? "Unknown error", Exception);
        }
    }
    /// Maps the result value to a new type if the operation was successful.
    /// <typeparam name="TNew">The new result type.</typeparam>
    /// <param name="mapper">Function to map the value.</param>
    /// <returns>A new result with the mapped value or the original failure.</returns>
    public ServiceResult<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        try
        {
            if (Value is null)
                return ServiceResult<TNew>.Failure("Value is null.");
            return ServiceResult<TNew>.Success(mapper(Value));
        }
        catch (Exception ex)
        {
            return ServiceResult<TNew>.Failure(ex);
        }
    }
}
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


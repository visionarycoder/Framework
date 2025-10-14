namespace VisionaryCoder.Framework;

/// <summary>
/// Result wrapper for framework operations that provides consistent success/failure handling.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public sealed class FrameworkResult<T>
{
    private FrameworkResult(bool isSuccess, T? value, string? errorMessage, Exception? exception)
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

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the result value if the operation was successful.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the exception if the operation failed with an exception.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <param name="value">The result value.</param>
    /// <returns>A successful result.</returns>
    public static FrameworkResult<T> Success(T value) => new(true, value, null, null);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed result.</returns>
    public static FrameworkResult<T> Failure(string errorMessage) => new(false, default, errorMessage, null);

    /// <summary>
    /// Creates a failed result with an exception.
    /// </summary>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A failed result.</returns>
    public static FrameworkResult<T> Failure(Exception exception) => new(false, default, exception.Message, exception);

    /// <summary>
    /// Creates a failed result with an error message and exception.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A failed result.</returns>
    public static FrameworkResult<T> Failure(string errorMessage, Exception exception) => new(false, default, errorMessage, exception);

    /// <summary>
    /// Matches the result and executes the appropriate action.
    /// </summary>
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

    /// <summary>
    /// Maps the result value to a new type if the operation was successful.
    /// </summary>
    /// <typeparam name="TNew">The new result type.</typeparam>
    /// <param name="mapper">Function to map the value.</param>
    /// <returns>A new result with the mapped value or the original failure.</returns>
    public FrameworkResult<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        if (IsSuccess && Value is not null)
        {
            try
            {
                var newValue = mapper(Value);
                return FrameworkResult<TNew>.Success(newValue);
            }
            catch (Exception ex)
            {
                return FrameworkResult<TNew>.Failure(ex);
            }
        }

        return Exception is not null 
            ? FrameworkResult<TNew>.Failure(ErrorMessage ?? "Unknown error", Exception)
            : FrameworkResult<TNew>.Failure(ErrorMessage ?? "Unknown error");
    }
}

/// <summary>
/// Non-generic result wrapper for operations that don't return a value.
/// </summary>
public sealed class FrameworkResult
{
    private FrameworkResult(bool isSuccess, string? errorMessage, Exception? exception)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the exception if the operation failed with an exception.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static FrameworkResult Success() => new(true, null, null);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed result.</returns>
    public static FrameworkResult Failure(string errorMessage) => new(false, errorMessage, null);

    /// <summary>
    /// Creates a failed result with an exception.
    /// </summary>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A failed result.</returns>
    public static FrameworkResult Failure(Exception exception) => new(false, exception.Message, exception);

    /// <summary>
    /// Creates a failed result with an error message and exception.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A failed result.</returns>
    public static FrameworkResult Failure(string errorMessage, Exception exception) => new(false, errorMessage, exception);

    /// <summary>
    /// Matches the result and executes the appropriate action.
    /// </summary>
    /// <param name="onSuccess">Action to execute if the result is successful.</param>
    /// <param name="onFailure">Action to execute if the result is a failure.</param>
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
}
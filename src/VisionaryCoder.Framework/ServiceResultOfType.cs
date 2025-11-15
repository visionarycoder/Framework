namespace VisionaryCoder.Framework;

/// <summary>
/// Result wrapper for framework operations that provides consistent success/failure handling.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>

public class ServiceResponse<T> : ServiceResponse
{

    private ServiceResponse(bool isSuccess, T? value, string? errorMessage, Exception? exception)
        : base(isSuccess, errorMessage, exception)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public new bool IsSuccess { get; } = false;

    /// Gets a value indicating whether the operation failed.
    public new bool IsFailure => !IsSuccess;

    /// Gets the result value if the operation was successful.
    public T? Value { get; }

    /// Gets the error message if the operation failed.
    public new string? ErrorMessage { get; } = null;

    /// Gets the exception if the operation failed with an exception.
    public new Exception? Exception { get; }

    /// Creates a successful result with a value.
    /// <param name="value">The result value.</param>
    /// <returns>A successful result.</returns>
    public static ServiceResponse<T> Success(T value) => new(true, value, null, null);

    /// Creates a failed result with an error message.
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed result.</returns>
    public static new ServiceResponse<T> Failure(string errorMessage) => new(false, default, errorMessage, null);

    /// Creates a failed result with an exception.
    /// <param name="exception">The exception that caused the failure.</param>
    public static new ServiceResponse<T> Failure(Exception exception) => new(false, default, exception.Message, exception);
    /// Creates a failed result with an error message and exception.
    public static new ServiceResponse<T> Failure(string errorMessage, Exception exception) => new(false, default, errorMessage, exception);

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
    public ServiceResponse<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        try
        {
            return Value is null
                ? ServiceResponse<TNew>.Failure("Value is null.")
                : ServiceResponse<TNew>.Success(mapper(Value));
        }
        catch (Exception ex)
        {
            return ServiceResponse<TNew>.Failure(ex);
        }
    }
}

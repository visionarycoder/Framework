namespace VisionaryCoder.Framework;

/// <summary>
/// Base result class for all operation outcomes.
/// </summary>
public abstract class ServiceResultBase(bool isSuccess, string? errorMessage, Exception? exception)
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; } = isSuccess;

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; } = errorMessage;

    /// <summary>
    /// Gets the exception if the operation failed with an exception.
    /// </summary>
    public Exception? Exception { get; } = exception;
}

/// <summary>
/// Result for operations that don't return a value.
/// </summary>
public sealed class ServiceResult : ServiceResultBase
{
    private ServiceResult(bool isSuccess, string? errorMessage, Exception? exception)
        : base(isSuccess, errorMessage, exception)
    {
    }

    public static ServiceResult Success() => new(true, null, null);
    public static ServiceResult Failure(string errorMessage) => new(false, errorMessage, null);
    public static ServiceResult Failure(Exception exception) => new(false, exception.Message, exception);
    public static ServiceResult Failure(string errorMessage, Exception exception) => new(false, errorMessage, exception);

    public void Match(Action onSuccess, Action<string, Exception?> onFailure)
    {
        if (IsSuccess)
            onSuccess();
        else
            onFailure(ErrorMessage ?? "Unknown error", Exception);
    }
}

/// <summary>
/// Result for operations that return a value.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public sealed class ServiceResult<T> : ServiceResultBase
{
    private ServiceResult(bool isSuccess, T? value, string? errorMessage, Exception? exception)
        : base(isSuccess, errorMessage, exception)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the result value if the operation was successful.
    /// </summary>
    public T? Value { get; }

    public static ServiceResult<T> Success(T value) => new(true, value, null, null);
    public static ServiceResult<T> Failure(string errorMessage) => new(false, default, errorMessage, null);
    public static ServiceResult<T> Failure(Exception exception) => new(false, default, exception.Message, exception);
    public static ServiceResult<T> Failure(string errorMessage, Exception exception) => new(false, default, errorMessage, exception);

    public void Match(Action<T> onSuccess, Action<string, Exception?> onFailure)
    {
        if (IsSuccess && Value is not null)
            onSuccess(Value);
        else
            onFailure(ErrorMessage ?? "Unknown error", Exception);
    }

    public ServiceResult<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        if (!IsSuccess || Value is null)
            return ServiceResult<TNew>.Failure(ErrorMessage ?? "Value is null");

        try
        {
            return ServiceResult<TNew>.Success(mapper(Value));
        }
        catch (Exception ex)
        {
            return ServiceResult<TNew>.Failure(ex);
        }
    }

    public async Task<ServiceResult<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapper)
    {
        if (!IsSuccess || Value is null)
            return ServiceResult<TNew>.Failure(ErrorMessage ?? "Value is null");

        try
        {
            TNew result = await mapper(Value);
            return ServiceResult<TNew>.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<TNew>.Failure(ex);
        }
    }
}

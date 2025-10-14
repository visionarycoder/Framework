// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Represents a response from a proxy operation.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public record Response<T>
{
    /// <summary>
    /// Gets or sets the response data.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Gets or sets the response value (alias for Data).
    /// </summary>
    public T? Value => Data;

    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets or sets the error if the operation failed.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Gets the error (alias for Exception).
    /// </summary>
    public Exception? Error => Exception;

    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage => Exception?.Message;

    /// <summary>
    /// Gets or sets the HTTP status code.
    /// </summary>
    public int StatusCode { get; init; } = 200;

    /// <summary>
    /// Gets or sets the correlation ID for tracking the request.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the duration of the operation.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    public static Response<T> Success(T data) => new() { Data = data, IsSuccess = true, StatusCode = 200 };

    /// <summary>
    /// Creates a successful response with status code.
    /// </summary>
    public static Response<T> Success(T data, int statusCode) => new() { Data = data, IsSuccess = true, StatusCode = statusCode };

    /// <summary>
    /// Creates a failed response from an exception.
    /// </summary>
    public static Response<T> Failure(Exception exception) => new() { IsSuccess = false, Exception = exception, StatusCode = 500 };

    /// <summary>
    /// Creates a failed response with error message.
    /// </summary>
    public static Response<T> Failure(string errorMessage) => new() { IsSuccess = false, Exception = new Exception(errorMessage), StatusCode = 500 };
}
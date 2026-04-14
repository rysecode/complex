using Complex.Application.Common.Errors;

namespace Complex.Application.Common.Responses;

public sealed class ApiResponse<T>
{
	public T? Data { get; init; }
	public ApiError? Error { get; init; }
	public string Message { get; init; } = "OK";

	public bool Success => Error is null;

	public static ApiResponse<T> Ok(T data, string? message = null)
		=> new() { Data = data, Message = message ?? "OK" };

	public static ApiResponse<T> Fail(ApiError error, string? message = null)
		=> new() { Error = error, Message = message ?? "Error" };
}

using Complex.Application.Common.Errors;

namespace Complex.Application.Common.Responses;

public class Result
{
	public bool Success { get; }
	public ErrorState? Error { get; }

	protected Result(bool success, ErrorState? error)
	{
		Success = success;
		Error = error;
	}

	public static Result Ok() => new(true, null);

	public static Result Fail(ErrorState error) => new(false, error);
}

public sealed class Result<T> : Result
{
	public T? Value { get; }

	private Result(bool success, T? value, ErrorState? error)
		: base(success, error)
	{
		Value = value;
	}

	public static Result<T> Ok(T value) => new(true, value, null);

	public new static Result<T> Fail(ErrorState error) => new(false, default, error);
}
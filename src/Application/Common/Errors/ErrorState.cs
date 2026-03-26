namespace Complex.Application.Common.Errors;

public sealed record ErrorState(string Code, string Message, IReadOnlyDictionary<string, object?>? Meta = null)
{
	public static ErrorState Validation(string message, IReadOnlyDictionary<string, object?>? meta = null)
		=> new(ErrorCodes.Validation, message, meta);

	public static ErrorState NotFound(string message, IReadOnlyDictionary<string, object?>? meta = null)
		=> new(ErrorCodes.NotFound, message, meta);

	public static ErrorState Conflict(string message, IReadOnlyDictionary<string, object?>? meta = null)
		=> new(ErrorCodes.Conflict, message, meta);

	public static ErrorState Unexpected(string message = "Unexpected error")
		=> new(ErrorCodes.Unexpected, message);
}

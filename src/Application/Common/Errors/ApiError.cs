namespace Complex.Application.Common.Errors;

public sealed class ApiError
{
	public string Code { get; init; } = ErrorCodes.Unexpected;
	public string Message { get; init; } = "Error";
	public IReadOnlyDictionary<string, object?>? Meta { get; init; }
	public string? TraceId { get; init; } // opcional: para suporte/observabilidade
}

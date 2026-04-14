namespace Complex.Application.Common.Responses;

public sealed class ReturnModel<TReturn>
{
	public TReturn? Return { get; set; }
	public ReturnError? Error { get; set; }
	public ReturnType Type { get; set; }

	public static ReturnModel<TReturn> Instance()
	{
		return new ReturnModel<TReturn>();
	}

	public void SetValidationError() => Type = ReturnType.ValidationError;

	public void SetSuccess() => Type = ReturnType.Success;

	public void SetSuccess(TReturn value)
	{
		Type = ReturnType.Success;
		Return = value;
	}

	public bool IsSuccess => Type == ReturnType.Success;
}

public sealed class ReturnError
{
	public string? Message { get; set; }
	public Exception? Exception { get; set; }
}

public enum ReturnType
{
	Success,
	Error,
	ValidationError
}
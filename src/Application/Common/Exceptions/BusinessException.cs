using Complex.Application.Common.Errors;

namespace Complex.Application.Common.Exceptions;

public class BusinessException : AppException
{
	public BusinessException() : base("Falha de regra de negócio") { }
	public BusinessException(string message) : base(message) { }
	public BusinessException(string message, Exception innerException) : base(message, innerException) { }
	public override ErrorState ToError() => ErrorState.Validation(Message);
}

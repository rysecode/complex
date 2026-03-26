using Complex.Application.Common.Errors;

namespace Complex.Application.Common.Exceptions;

public abstract class AppException : Exception
{
	protected AppException(string message) : base(message) { }
	protected AppException(string message, Exception innerException) : base(message, innerException) { }
	public abstract ErrorState ToError();
}

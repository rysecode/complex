using Complex.Application.Common.Errors;

namespace Complex.Application.Common.Exceptions;

public abstract class IntegrationException : Exception
{
	protected IntegrationException(string message) : base(message) { }
	protected IntegrationException(string message, Exception innerException) : base(message, innerException) { }
	public abstract ErrorState ToError();
}

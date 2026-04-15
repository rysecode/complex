using Complex.Application.Common.Errors;

namespace Complex.Application.Common.Exceptions;

public sealed class ComponentException : AppException
{
	public ComponentException() : base("Falha realizar o processamento de uma biblioteca dependente.") { }
	public ComponentException(string message) : base(message) { }
	public ComponentException(string message, Exception innerException) : base(message, innerException) { }
	public override ErrorState ToError() => ErrorState.Validation(Message);
}

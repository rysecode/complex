using Complex.Application.Common.Errors;

namespace Complex.Application.Common.Exceptions;

public sealed class DomainException : AppException
{
	public DomainException() : base("Erro na validação de dados") { }
	public DomainException(string message) : base(message) { }
	public DomainException(string message, Exception innerException) : base(message, innerException) { }
	public override ErrorState ToError() => ErrorState.Validation(Message);
}

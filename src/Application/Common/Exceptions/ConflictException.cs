using Complex.Application.Common.Errors;

namespace Complex.Application.Common.Exceptions;

public sealed class ConflictException : AppException
{
	public ConflictException() : base("Registro não encontrado.") { }

	public ConflictException(string message) : base(message) { }

	public ConflictException(string message, Exception innerException) : base(message, innerException) { }

	public override ErrorState ToError() => ErrorState.Conflict(Message);
}

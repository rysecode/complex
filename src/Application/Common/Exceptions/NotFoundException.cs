using Complex.Application.Common.Errors;

namespace Complex.Application.Common.Exceptions;

public sealed class NotFoundException : AppException
{
	public NotFoundException() : base("Registro não encontrado.") { }

	public NotFoundException(string message) : base(message) { }

	public NotFoundException(string message, Exception innerException) : base(message, innerException) { }

	public override ErrorState ToError() => ErrorState.NotFound(Message);
}

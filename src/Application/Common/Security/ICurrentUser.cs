namespace Complex.Application.Common.Security;

public interface ICurrentUser
{
	bool IsAuthenticated { get; }

	Guid UsuarioId { get; }
	Guid EmpresaId { get; }
	Guid PessoaId { get; }

	string Email { get; }
}

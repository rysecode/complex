using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Complex.Application.Common.Security;

public sealed class HttpContextCurrentUser : ICurrentUser
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

	public bool IsAuthenticated
		=> User?.Identity?.IsAuthenticated == true;

	public Guid UsuarioId => GetGuidClaim("usuario_id");

	public Guid EmpresaId => GetGuidClaim("empresa_id");

	public Guid PessoaId => GetGuidClaim("pessoa_id");

	public string Email => GetRequiredClaim(ClaimTypes.Email);

	// ------------------------------------------------------------
	// Helpers internos
	// ------------------------------------------------------------

	private Guid GetGuidClaim(string claimType)
	{
		if (!IsAuthenticated)
			throw new UnauthorizedAccessException("Usuário não autenticado.");

		var value = User!.FindFirst(claimType)?.Value;

		if (string.IsNullOrWhiteSpace(value))
			throw new UnauthorizedAccessException($"Claim '{claimType}' não encontrada no token.");

		if (!Guid.TryParse(value, out var guid))
			throw new UnauthorizedAccessException($"Claim '{claimType}' inválida no token.");

		return guid;
	}

	private string GetRequiredClaim(string claimType)
	{
		if (!IsAuthenticated)
			throw new UnauthorizedAccessException("Usuário não autenticado.");

		var value = User!.FindFirst(claimType)?.Value;

		if (string.IsNullOrWhiteSpace(value))
			throw new UnauthorizedAccessException($"Claim '{claimType}' não encontrada no token.");

		return value;
	}
}

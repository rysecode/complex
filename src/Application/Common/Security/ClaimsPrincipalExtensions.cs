using System.Security.Claims;

namespace Complex.Application.Common.Security;

public static class ClaimsPrincipalExtensions
{
	// ------------------------------------------------------------
	// Básico
	// ------------------------------------------------------------

	public static Guid GetUsuarioId(this ClaimsPrincipal user)
		=> GetGuidClaim(user, "usuario_id");

	public static Guid GetEmpresaId(this ClaimsPrincipal user)
		=> GetGuidClaim(user, "empresa_id");

	public static Guid GetPessoaId(this ClaimsPrincipal user)
		=> GetGuidClaim(user, "pessoa_id");

	public static string GetEmail(this ClaimsPrincipal user)
		=> GetRequiredClaim(user, ClaimTypes.Email);

	// ------------------------------------------------------------
	// Helpers internos
	// ------------------------------------------------------------

	private static Guid GetGuidClaim(ClaimsPrincipal user, string claimType)
	{
		var value = user.FindFirst(claimType)?.Value;

		if (string.IsNullOrWhiteSpace(value))
			throw new UnauthorizedAccessException($"Claim '{claimType}' não encontrada no token.");

		if (!Guid.TryParse(value, out var guid))
			throw new UnauthorizedAccessException($"Claim '{claimType}' inválida no token.");

		return guid;
	}

	private static string GetRequiredClaim(ClaimsPrincipal user, string claimType)
	{
		var value = user.FindFirst(claimType)?.Value;

		if (string.IsNullOrWhiteSpace(value))
			throw new UnauthorizedAccessException($"Claim '{claimType}' não encontrada no token.");

		return value;
	}
}

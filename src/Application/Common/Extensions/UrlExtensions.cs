namespace Complex.Application.Common.Extensions;

public static class UrlExtensions
{
	public static bool IsValidUrlOuUser(this string? value)
	{
		if (string.IsNullOrWhiteSpace(value)) return false;

		// Valida se é uma URL ou se tem pelo menos 3 caracteres (para usernames)
		return Uri.TryCreate(value, UriKind.Absolute, out _) || value.Length >= 3;
	}
}

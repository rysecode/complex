using System.Text.RegularExpressions;

namespace Complex.Application.Common.Extensions;

public static class AddressExtension
{
	public static bool IsValidCep(this string? value)
	{
		if (string.IsNullOrWhiteSpace(value)) return false;

		// Remove máscaras (hifens ou pontos)
		var digits = Regex.Replace(value, @"[^\d]", "");
		return digits.Length == 8;
	}
}

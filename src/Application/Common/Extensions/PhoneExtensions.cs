namespace Complex.Application.Common.Extensions;

public static class PhoneExtensions
{
	public static bool IsValidPhone(this string? value)
	{
		// Remove caracteres não numéricos e valida se tem entre 10 e 11 dígitos
		var digits = value.OnlyDigits();
		return digits.Length >= 10 && digits.Length <= 11;
	}
}

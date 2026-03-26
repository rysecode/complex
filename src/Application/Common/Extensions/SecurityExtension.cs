namespace Complex.Application.Common.Extensions;

public static class SecurityExtension
{
	public static bool IsValidBasicPassword(this string? value)
	{
		if (string.IsNullOrWhiteSpace(value)) return false;
		if (value.Length < 8) return false;

		return true;
	}
}

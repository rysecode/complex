using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Complex.Application.Common.Extensions;

public static class EmailExtension
{
	// ---------------------------------------------------------------------
	// EMAIL
	// ---------------------------------------------------------------------	
	public static bool IsValidEmail(this string? value)
	{
		if (string.IsNullOrWhiteSpace(value)) return false;

		var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
		if (!regex.IsMatch(value)) return false;
		try { _ = new MailAddress(value.Trim()); return true; }
		catch { return false; }
	}
}

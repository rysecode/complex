using System.Text.RegularExpressions;

namespace Complex.Application.Common.Extensions;

public static class DocumentExtensions
{
	// ---------------------------------------------------------------------
	// Helpers
	// ---------------------------------------------------------------------
	public static string OnlyDigits(this string? value)
		=> new string((value ?? string.Empty).Where(char.IsDigit).ToArray());

	public static string NormalizeCpf(this string? value)
	{
		if (string.IsNullOrWhiteSpace(value)) return string.Empty;
		return Regex.Replace(value, "[^0-9]", "");
	}

	public static string NormalizeCnpj(this string? value)
	{
		if (string.IsNullOrWhiteSpace(value)) return string.Empty;
		return Regex.Replace(value, "[^0-9]", "");
	}

	// ---------------------------------------------------------------------
	// CPF
	// ---------------------------------------------------------------------
	public static bool IsValidCpf(this string? value)
	{
		var cpf = value.OnlyDigits();

		if (cpf.Length != 11)
			return false;

		// Evita CPFs com todos os dígitos iguais
		if (cpf.All(c => c == cpf[0]))
			return false;

		var numbers = cpf.Select(c => c - '0').ToArray();

		// Primeiro dígito
		var sum = 0;
		for (int i = 0; i < 9; i++)
			sum += numbers[i] * (10 - i);

		var remainder = sum % 11;
		var digit1 = remainder < 2 ? 0 : 11 - remainder;

		if (numbers[9] != digit1)
			return false;

		// Segundo dígito
		sum = 0;
		for (int i = 0; i < 10; i++)
			sum += numbers[i] * (11 - i);

		remainder = sum % 11;
		var digit2 = remainder < 2 ? 0 : 11 - remainder;

		return numbers[10] == digit2;
	}

	// ---------------------------------------------------------------------
	// CNPJ
	// ---------------------------------------------------------------------
	public static bool IsValidCnpj(this string? value)
	{
		var cnpj = value.OnlyDigits();

		if (cnpj.Length != 14)
			return false;

		// Evita CNPJs com todos os dígitos iguais
		if (cnpj.All(c => c == cnpj[0]))
			return false;

		var numbers = cnpj.Select(c => c - '0').ToArray();

		int[] weights1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
		int[] weights2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

		// Primeiro dígito
		var sum = 0;
		for (int i = 0; i < 12; i++)
			sum += numbers[i] * weights1[i];

		var remainder = sum % 11;
		var digit1 = remainder < 2 ? 0 : 11 - remainder;

		if (numbers[12] != digit1)
			return false;

		// Segundo dígito
		sum = 0;
		for (int i = 0; i < 13; i++)
			sum += numbers[i] * weights2[i];

		remainder = sum % 11;
		var digit2 = remainder < 2 ? 0 : 11 - remainder;

		return numbers[13] == digit2;
	}
}

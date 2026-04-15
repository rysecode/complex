using Complex.Application.Common.Exceptions;
using System.Security.Cryptography;

namespace Complex.Application.Common.Hashers;

public static class PasswordHasher
{
	private const int SaltSize = 16;      // 128-bit
	private const int KeySize = 32;       // 256-bit
	private const int Iterations = 100_000;

	public static (string HashBase64, string SaltBase64) Hash(string password)
	{
		if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
			throw new DomainException("Senha inválida. Mínimo de 8 caracteres.");

		var salt = RandomNumberGenerator.GetBytes(SaltSize);

		var key = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			Iterations,
			HashAlgorithmName.SHA256,
			KeySize);

		return (Convert.ToBase64String(key), Convert.ToBase64String(salt));
	}

	public static bool Verify(string password, string hashBase64, string saltBase64)
	{
		var salt = Convert.FromBase64String(saltBase64);
		var expected = Convert.FromBase64String(hashBase64);

		var actual = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			Iterations,
			HashAlgorithmName.SHA256,
			expected.Length);

		return CryptographicOperations.FixedTimeEquals(actual, expected);
	}
}

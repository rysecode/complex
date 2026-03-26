using Microsoft.AspNetCore.Http;
using System.Text;

namespace Complex.Application.Common.Helpers;

public static class AvatarHelper
{
	public static string? ToDataUri(string? base64, string? mimeType)
	{
		if (string.IsNullOrWhiteSpace(base64))
			return null;

		var mime = string.IsNullOrWhiteSpace(mimeType)
			? "image/png"
			: mimeType.Trim().ToLowerInvariant();

		return $"data:{mime};base64,{base64}";
	}

	/// <summary>
	/// Converte um IFormFile em (Base64Bytes, MimeType).
	/// Base64Bytes = bytes do TEXTO base64 (NÃO é o arquivo raw).
	/// </summary>
	public static async Task<(byte[] Base64Bytes, string MimeType)> FromFormFileAsync(IFormFile file, long maxBytes = 2 * 1024 * 1024, CancellationToken ct = default)
	{
		if (file is null) throw new ArgumentNullException(nameof(file));
		if (file.Length <= 0) throw new ArgumentException("Arquivo vazio.", nameof(file));
		if (file.Length > maxBytes) throw new ArgumentException($"Arquivo excede o limite de {maxBytes} bytes.", nameof(file));

		var mimeType = string.IsNullOrWhiteSpace(file.ContentType)
			? "application/octet-stream"
			: file.ContentType.Trim().ToLowerInvariant();

		// 1) bytes reais do arquivo
		await using var ms = new MemoryStream(capacity: (int)Math.Min(file.Length, int.MaxValue));
		await file.CopyToAsync(ms, ct);
		var rawBytes = ms.ToArray();

		// 2) Base64 UMA vez
		var base64Text = Convert.ToBase64String(rawBytes);

		// 3) bytes do texto Base64
		var base64Bytes = Encoding.UTF8.GetBytes(base64Text);

		return (base64Bytes, mimeType);
	}

	public static async Task<(byte[] RawBytes, string MimeType)> ReadRawAsync(IFormFile file, long maxBytes = 2 * 1024 * 1024, CancellationToken ct = default)
	{
		if (file is null) throw new ArgumentNullException(nameof(file));
		if (file.Length <= 0) throw new ArgumentException("Arquivo vazio.", nameof(file));
		if (file.Length > maxBytes) throw new ArgumentException($"Arquivo excede o limite de {maxBytes} bytes.", nameof(file));

		var mimeType = string.IsNullOrWhiteSpace(file.ContentType)
			? "application/octet-stream"
			: file.ContentType.Trim().ToLowerInvariant();

		await using var ms = new MemoryStream(capacity: (int)Math.Min(file.Length, int.MaxValue));
		await file.CopyToAsync(ms, ct);
		return (ms.ToArray(), mimeType);
	}
}

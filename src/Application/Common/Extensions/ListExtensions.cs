namespace Complex.Application.Common.Extensions;

public static class ListExtensions
{
	/// <summary>
	/// Transforma uma lista de mensagens de erro em um dicionário onde a chave é o nome do atributo.
	/// </summary>
	/// <param name="errors">A lista de strings (erros).</param>
	/// <param name="attributeName">O nome do atributo/campo ao qual os erros pertencem.</param>
	/// <returns>Um dicionário formatado para respostas de erro (como ValidationProblemDetails).</returns>
	public static Dictionary<string, string[]> ToDictionary(this List<string> errors, string attributeName)
	{
		if (errors == null || !errors.Any())
		{
			return new Dictionary<string, string[]>();
		}

		return new Dictionary<string, string[]>
		{
			{ attributeName, errors.ToArray() }
		};
	}


	public static Dictionary<string, object?> ToDictionaryObject(this List<string> errors, string attributeName)
	{
		if (errors == null || !errors.Any())
		{
			return new Dictionary<string, object?>();
		}

		// Retornamos como object? para garantir compatibilidade com IReadOnlyDictionary<string, object?>
		return new Dictionary<string, object?>
		{
			{ attributeName, errors.ToArray() }
		};
	}

}

using System.Collections;

namespace Complex.Application.Common.Extensions;

public static class DictionaryExtensions
{
	/// <summary>
	/// Converte um IDictionary para IReadOnlyDictionary<string, object?>
	/// </summary>
	/// <param name="dictionary">O dicionário a ser convertido</param>
	/// <returns>Uma visão somente leitura do dicionário</returns>
	/// <exception cref="ArgumentNullException">Lançada se o dicionário for null</exception>
	/// <exception cref="InvalidCastException">Lançada se as chaves não forem strings</exception>
	public static IReadOnlyDictionary<string, object?> ToReadOnlyDictionary(this IDictionary dictionary)
	{
		if (dictionary == null)
			throw new ArgumentNullException(nameof(dictionary));

		// Se já for do tipo desejado, retorna diretamente
		if (dictionary is IReadOnlyDictionary<string, object?> readOnlyDict)
			return readOnlyDict;

		// Converte para Dictionary<string, object?> primeiro
		var convertedDict = new Dictionary<string, object?>();

		foreach (DictionaryEntry entry in dictionary)
		{
			if (entry.Key is not string key)
				throw new InvalidCastException($"Chave do tipo {entry.Key?.GetType()} não pode ser convertida para string");

			convertedDict[key] = entry.Value;
		}

		return convertedDict;
	}

	/// <summary>
	/// Versão genérica para IDictionary<TKey, TValue>
	/// </summary>
	public static IReadOnlyDictionary<string, object?> ToReadOnlyDictionary<TKey, TValue>(
		this IDictionary<TKey, TValue> dictionary)
	{
		if (dictionary == null)
			throw new ArgumentNullException(nameof(dictionary));

		// Se já for do tipo desejado, retorna
		if (dictionary is IReadOnlyDictionary<string, object?> readOnlyDict)
			return readOnlyDict;

		// Converte verificando o tipo da chave
		if (typeof(TKey) != typeof(string))
		{
			// Tenta converter as chaves para string
			return dictionary.ToDictionary(
				kvp => kvp.Key?.ToString() ?? throw new InvalidOperationException("Chave não pode ser null"),
				kvp => (object?)kvp.Value
			);
		}

		// Cast seguro quando TKey é string
		return dictionary.ToDictionary(
			kvp => (string)(object)kvp.Key,
			kvp => (object?)kvp.Value
		);
	}

	/// <summary>
	/// Versão otimizada que retorna uma wrapper sem criar uma nova cópia
	/// </summary>
	public static IReadOnlyDictionary<string, object?> AsReadOnly(this IDictionary dictionary)
	{
		return dictionary == null ? throw new ArgumentNullException(nameof(dictionary)) : (IReadOnlyDictionary<string, object?>)new ReadOnlyDictionaryWrapper(dictionary);
	}

	/// <summary>
	/// Wrapper que implementa IReadOnlyDictionary sem copiar os dados
	/// </summary>
	private class ReadOnlyDictionaryWrapper : IReadOnlyDictionary<string, object?>
	{
		private readonly IDictionary _source;

		public ReadOnlyDictionaryWrapper(IDictionary source)
		{
			_source = source;
		}

		public object? this[string key] => _source.Contains(key) ? _source[key] : throw new KeyNotFoundException();

		public IEnumerable<string> Keys => _source.Keys.Cast<string>();
		public IEnumerable<object?> Values => _source.Values.Cast<object?>();
		public int Count => _source.Count;

		public bool ContainsKey(string key) => _source.Contains(key);

		public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
		{
			foreach (DictionaryEntry entry in _source)
			{
				yield return entry.Key is string key
					? new KeyValuePair<string, object?>(key, entry.Value)
					: throw new InvalidOperationException($"Chave do tipo {entry.Key?.GetType()} não é string");
			}
		}

		public bool TryGetValue(string key, out object? value)
		{
			if (_source.Contains(key))
			{
				value = _source[key];
				return true;
			}

			value = null;
			return false;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
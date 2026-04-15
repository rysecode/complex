namespace Complex.Application.Common.Builders;

public abstract class Builder<T> : IBuilder<T> where T : class
{
	protected T Instance { get; private set; }

	protected Builder()
	{
		Instance = CreateInstance();
	}

	protected Builder(T instance)
	{
		Instance = instance ?? throw new ArgumentNullException(nameof(instance));
	}

	protected static T CreateInstance()
	{
		return Activator.CreateInstance<T>() is T instance ? instance : throw new InvalidOperationException($"Não foi possível criar uma instância de {typeof(T).FullName}.");
	}

	public virtual T Build()
	{
		// Validações básicas podem ser adicionadas aqui
		return Instance;
	}
}

namespace Complex.Application.Common.Builders;

public interface IBuilder<out T> where T : class
{
	T Build();
}

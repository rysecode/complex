using Complex.Application.Common.Exceptions;
using Complex.Application.Common.Responses;

namespace Complex.Application.Common.Models;

public abstract class Domains : IDomains
{
	protected ReturnModel<List<string>> _instance = ReturnModel<List<string>>.Instance();
	protected List<string> _errors = new();
	private const string ERRO_MESSAGE = "Error na validação do dominio.";

	public ReturnModel<List<string>> CheckValidade()
	{
		if (_errors.Count == 0) _instance.SetSuccess(_errors);
		else
		{
			_instance.SetValidationError();
			_instance.Return = _errors;
			_instance.Error = new ReturnError
			{
				Message = ERRO_MESSAGE,
				Exception = new DomainException(ERRO_MESSAGE)
			};
		}

		return _instance;
	}

	public void AddError(string error)
	{
		_errors.Add(error);
	}

	protected void ClearValidation()
	{
		if (_errors is null) _errors = new();
		else _errors.Clear();
	}

	public virtual ReturnModel<List<string>> Validate()
	{
		return _instance;
	}
}

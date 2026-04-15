using Complex.Application.Common.Responses;

namespace Complex.Application.Common.Models;

public interface IDomains
{
	ReturnModel<List<string>> Validate();
}

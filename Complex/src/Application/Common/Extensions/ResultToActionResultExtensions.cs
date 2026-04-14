using Complex.Application.Common.Errors;
using Complex.Application.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Complex.Application.Common.Extensions;

public static class ResultToActionResultExtensions
{
	public static ActionResult<ApiResponse<T>> ToActionResult<T>(this ControllerBase controller, Result<T> result, Func<T, object>? mapData = null) // se quiser mapear DTO sem AutoMapper
	{
		if (result.Success)
		{
			var payload = mapData is null ? result.Value! : (T)mapData(result.Value!);
			return controller.Ok(ApiResponse<T>.Ok(payload));
		}

		var (statusCode, apiError) = MapError(result.Error!, controller.HttpContext.TraceIdentifier);

		return controller.StatusCode(statusCode, ApiResponse<T>.Fail(apiError));
	}

	public static ActionResult<ApiResponse<object>> ToActionResult(this ControllerBase controller, Result result)
	{
		if (result.Success)
			return controller.Ok(ApiResponse<object>.Ok(new { }));

		var (statusCode, apiError) = MapError(result.Error!, controller.HttpContext.TraceIdentifier);
		return controller.StatusCode(statusCode, ApiResponse<object>.Fail(apiError));
	}

	public static IActionResult ToIActionResult<T>(this ControllerBase controller, HttpStatusCode responseCode, Result<T> result, Func<T, object>? mapData = null)
	{
		if (result.Success)
		{
			var payload = mapData is null ? result.Value! : (T)mapData(result.Value!);
			return controller.StatusCode((int)responseCode, payload);
		}

		var (statusCode, apiError) = MapError(result.Error!, controller.HttpContext.TraceIdentifier);

		return controller.StatusCode(statusCode, ApiResponse<T>.Fail(apiError));
	}

	private static int GetStatus(string status)
	{
		return status switch
		{
			ErrorCodes.Validation => 400,
			ErrorCodes.NotFound => 404,
			ErrorCodes.Conflict => 409,
			ErrorCodes.Forbidden => 403,
			ErrorCodes.Unauthorized => 401,
			_ => 500
		};
	}

	private static (int statusCode, ApiError apiError) MapError(ErrorState error, string traceId)
	{
		var status = GetStatus(error.Code);

		var apiError = new ApiError
		{
			Code = error.Code,
			Message = error.Message,
			Meta = error.Meta,
			TraceId = traceId
		};

		return (status, apiError);
	}
}

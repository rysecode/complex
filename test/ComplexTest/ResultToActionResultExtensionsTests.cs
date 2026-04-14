using Complex.Application.Common.Errors;
using Complex.Application.Common.Extensions;
using Complex.Application.Common.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ComplexTest;

public class ResultToActionResultExtensionsTests
{
    [Fact]
    public void ToActionResultGeneric_WhenSuccess_ShouldReturnOkApiResponse()
    {
        var controller = CreateController();
        var payload = new SampleDto { Id = 7, Name = "ryse" };
        var result = Result<SampleDto>.Ok(payload);

        var actionResult = controller.ToActionResult(result);

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<SampleDto>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(7, response.Data!.Id);
        Assert.Equal("ryse", response.Data.Name);
    }

    [Fact]
    public void ToActionResultGeneric_WithMapData_ShouldUseMappedValue()
    {
        var controller = CreateController();
        var result = Result<SampleDto>.Ok(new SampleDto { Id = 1, Name = "before" });

        var actionResult = controller.ToActionResult(
            result,
            item => new SampleDto { Id = item.Id, Name = "after" });

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<SampleDto>>(ok.Value);
        Assert.Equal("after", response.Data!.Name);
    }

    [Fact]
    public void ToActionResultGeneric_WhenFailure_ShouldMapErrorAndTraceId()
    {
        var controller = CreateController("trace-404");
        var result = Result<SampleDto>.Fail(ErrorState.NotFound("missing record"));

        var actionResult = controller.ToActionResult(result);

        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<SampleDto>>(objectResult.Value);
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        Assert.False(response.Success);
        Assert.Equal(ErrorCodes.NotFound, response.Error!.Code);
        Assert.Equal("missing record", response.Error.Message);
        Assert.Equal("trace-404", response.Error.TraceId);
    }

    [Fact]
    public void ToActionResultNonGeneric_WhenSuccess_ShouldReturnEmptyPayload()
    {
        var controller = CreateController();

        var actionResult = controller.ToActionResult(Result.Ok());

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<object>>(ok.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    [Fact]
    public void ToIActionResult_WhenSuccess_ShouldUseProvidedStatusCode()
    {
        var controller = CreateController();
        var payload = new SampleDto { Id = 11, Name = "created" };

        var actionResult = controller.ToIActionResult(HttpStatusCode.Created, Result<SampleDto>.Ok(payload));

        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
        var body = Assert.IsType<SampleDto>(objectResult.Value);
        Assert.Equal("created", body.Name);
    }

    [Fact]
    public void ToIActionResult_WhenFailure_ShouldReturnStandardApiErrorResponse()
    {
        var controller = CreateController("trace-409");
        var result = Result<SampleDto>.Fail(ErrorState.Conflict("already exists"));

        var actionResult = controller.ToIActionResult(HttpStatusCode.Created, result);

        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        var response = Assert.IsType<ApiResponse<SampleDto>>(objectResult.Value);
        Assert.Equal(StatusCodes.Status409Conflict, objectResult.StatusCode);
        Assert.Equal(ErrorCodes.Conflict, response.Error!.Code);
        Assert.Equal("trace-409", response.Error.TraceId);
    }

    private static TestController CreateController(string traceIdentifier = "trace-123")
    {
        var httpContext = new DefaultHttpContext
        {
            TraceIdentifier = traceIdentifier
        };

        return new TestController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    private sealed class TestController : ControllerBase
    {
    }

    private sealed class SampleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

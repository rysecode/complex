using Complex.Application.Common.Errors;
using Complex.Application.Common.Exceptions;
using Complex.Application.Common.Responses;

namespace ComplexTest;

public class ResponsesAndErrorsTests
{
    [Fact]
    public void ResultOk_ShouldCreateSuccessfulResult()
    {
        var result = Result.Ok();

        Assert.True(result.Success);
        Assert.Null(result.Error);
    }

    [Fact]
    public void ResultFail_ShouldStoreError()
    {
        var error = ErrorState.Validation("invalid data");

        var result = Result.Fail(error);

        Assert.False(result.Success);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void GenericResultOk_ShouldStoreValue()
    {
        var payload = new SamplePayload { Id = 10, Name = "alpha" };

        var result = Result<SamplePayload>.Ok(payload);

        Assert.True(result.Success);
        Assert.Same(payload, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void GenericResultFail_ShouldStoreErrorAndDefaultValue()
    {
        var error = ErrorState.NotFound("missing");

        var result = Result<SamplePayload>.Fail(error);

        Assert.False(result.Success);
        Assert.Null(result.Value);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void ApiResponseOk_ShouldExposeSuccessAndData()
    {
        var payload = new SamplePayload { Id = 1, Name = "ok" };

        var response = ApiResponse<SamplePayload>.Ok(payload, "done");

        Assert.True(response.Success);
        Assert.Same(payload, response.Data);
        Assert.Equal("done", response.Message);
        Assert.Null(response.Error);
    }

    [Fact]
    public void ApiResponseFail_ShouldExposeError()
    {
        var error = new ApiError
        {
            Code = ErrorCodes.Conflict,
            Message = "duplicate"
        };

        var response = ApiResponse<SamplePayload>.Fail(error, "failed");

        Assert.False(response.Success);
        Assert.Equal(error, response.Error);
        Assert.Equal("failed", response.Message);
        Assert.Null(response.Data);
    }

    [Fact]
    public void ErrorStateFactories_ShouldUseExpectedCodes()
    {
        Assert.Equal(ErrorCodes.Validation, ErrorState.Validation("validation").Code);
        Assert.Equal(ErrorCodes.NotFound, ErrorState.NotFound("not found").Code);
        Assert.Equal(ErrorCodes.Conflict, ErrorState.Conflict("conflict").Code);
        Assert.Equal(ErrorCodes.Unexpected, ErrorState.Unexpected().Code);
    }

    [Fact]
    public void ExceptionsShouldConvertToExpectedErrors()
    {
        Assert.Equal(ErrorCodes.Validation, new BusinessException("rule").ToError().Code);
        Assert.Equal(ErrorCodes.Validation, new DomainException("domain").ToError().Code);
        Assert.Equal(ErrorCodes.Validation, new ComponentException("component").ToError().Code);
        Assert.Equal(ErrorCodes.Conflict, new ConflictException("conflict").ToError().Code);
        Assert.Equal(ErrorCodes.NotFound, new NotFoundException("missing").ToError().Code);
    }

    [Fact]
    public void ReturnModelShouldExposeStateTransitions()
    {
        var model = ReturnModel<string>.Instance();

        model.SetSuccess("value");

        Assert.True(model.IsSuccess);
        Assert.Equal(ReturnType.Success, model.Type);
        Assert.Equal("value", model.Return);

        model.SetValidationError();

        Assert.False(model.IsSuccess);
        Assert.Equal(ReturnType.ValidationError, model.Type);
    }

    private sealed class SamplePayload
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

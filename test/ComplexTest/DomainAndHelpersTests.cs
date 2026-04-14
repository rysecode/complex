using Complex.Application.Common.Builders;
using Complex.Application.Common.Helpers;
using Complex.Application.Common.Libs;
using Complex.Application.Common.Models;
using Complex.Application.Common.Responses;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace ComplexTest;

public class DomainAndHelpersTests
{
    [Fact]
    public void DomainsValidate_ShouldReturnSuccessWhenNoErrors()
    {
        var domain = new SampleDomain("valid");

        var result = domain.Validate();

        Assert.True(result.IsSuccess);
        Assert.Equal(ReturnType.Success, result.Type);
        Assert.Empty(result.Return!);
        Assert.Null(result.Error);
    }

    [Fact]
    public void DomainsValidate_ShouldReturnValidationErrorWhenErrorsExist()
    {
        var domain = new SampleDomain(string.Empty);

        var result = domain.Validate();

        Assert.False(result.IsSuccess);
        Assert.Equal(ReturnType.ValidationError, result.Type);
        Assert.NotNull(result.Error);
        Assert.Contains("name is required", result.Return!);
    }

    [Fact]
    public void BuilderBuild_ShouldReturnConfiguredInstance()
    {
        var model = new SampleBuilder()
            .WithName("complex")
            .Build();

        Assert.Equal("complex", model.Name);
    }

    [Fact]
    public void PagedResultConstructor_ShouldAssignValues()
    {
        var items = new List<string> { "a", "b" };

        var page = new PagedResult<string>(items, total: 5, page: 2, pageSize: 10);

        Assert.Equal(2, page.Page);
        Assert.Equal(10, page.PageSize);
        Assert.Equal(5, page.Total);
        Assert.Equal(items, page.Items);
    }

    [Fact]
    public async Task AvatarHelperFromFormFileAsync_ShouldReturnBase64AndMimeType()
    {
        var content = Encoding.UTF8.GetBytes("avatar-content");
        await using var stream = new MemoryStream(content);
        IFormFile file = new FormFile(stream, 0, content.Length, "avatar", "avatar.txt")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        var (base64Bytes, mimeType) = await AvatarHelper.FromFormFileAsync(file);

        Assert.Equal("text/plain", mimeType);
        Assert.Equal(Convert.ToBase64String(content), Encoding.UTF8.GetString(base64Bytes));
    }

    [Fact]
    public async Task AvatarHelperReadRawAsync_ShouldReturnOriginalBytes()
    {
        var content = Encoding.UTF8.GetBytes("raw-avatar");
        await using var stream = new MemoryStream(content);
        IFormFile file = new FormFile(stream, 0, content.Length, "avatar", "avatar.bin")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octet-stream"
        };

        var (rawBytes, mimeType) = await AvatarHelper.ReadRawAsync(file);

        Assert.Equal("application/octet-stream", mimeType);
        Assert.Equal(content, rawBytes);
    }

    [Fact]
    public void AvatarHelperToDataUri_ShouldUseDefaultMimeWhenNotProvided()
    {
        var result = AvatarHelper.ToDataUri("YWJj", null);

        Assert.Equal("data:image/png;base64,YWJj", result);
        Assert.Null(AvatarHelper.ToDataUri(null, "image/jpeg"));
    }

    [Fact]
    public void GeoMathBearingDegrees_ShouldReturnExpectedCardinalDirections()
    {
        var north = GeoMath.BearingDegrees(0m, 0m, 1m, 0m);
        var east = GeoMath.BearingDegrees(0m, 0m, 0m, 1m);

        Assert.InRange(north, -0.0001m, 0.0001m);
        Assert.InRange(east, 89.9m, 90.1m);
    }

    private sealed class SampleDomain : Domains
    {
        private readonly string _name;

        public SampleDomain(string name)
        {
            _name = name;
        }

        public override ReturnModel<List<string>> Validate()
        {
            ClearValidation();

            if (string.IsNullOrWhiteSpace(_name))
            {
                AddError("name is required");
            }

            return CheckValidade();
        }
    }

    private sealed class SampleBuilder : Builder<SampleModel>
    {
        public SampleBuilder WithName(string name)
        {
            Instance.Name = name;
            return this;
        }
    }

    private sealed class SampleModel
    {
        public string Name { get; set; } = string.Empty;
    }
}

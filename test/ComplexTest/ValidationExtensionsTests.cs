using Complex.Application.Common.Extensions;

namespace ComplexTest;

public class ValidationExtensionsTests
{
    [Fact]
    public void OnlyDigits_ShouldStripNonDigits()
    {
        Assert.Equal("12345678900", "123.456.789-00".OnlyDigits());
    }

    [Fact]
    public void NormalizeDocumentMethods_ShouldStripMasks()
    {
        Assert.Equal("52998224725", "529.982.247-25".NormalizeCpf());
        Assert.Equal("12345678000195", "12.345.678/0001-95".NormalizeCnpj());
    }

    [Theory]
    [InlineData("529.982.247-25", true)]
    [InlineData("111.111.111-11", false)]
    [InlineData("123", false)]
    public void IsValidCpf_ShouldValidateExpectedCases(string value, bool expected)
    {
        Assert.Equal(expected, value.IsValidCpf());
    }

    [Theory]
    [InlineData("12.345.678/0001-95", true)]
    [InlineData("11.111.111/1111-11", false)]
    [InlineData("123", false)]
    public void IsValidCnpj_ShouldValidateExpectedCases(string value, bool expected)
    {
        Assert.Equal(expected, value.IsValidCnpj());
    }

    [Theory]
    [InlineData("user@example.com", true)]
    [InlineData("invalid-email", false)]
    [InlineData("", false)]
    public void IsValidEmail_ShouldValidateExpectedCases(string value, bool expected)
    {
        Assert.Equal(expected, value.IsValidEmail());
    }

    [Theory]
    [InlineData("(11) 98765-4321", true)]
    [InlineData("1132654321", true)]
    [InlineData("12345", false)]
    public void IsValidPhone_ShouldValidateExpectedCases(string value, bool expected)
    {
        Assert.Equal(expected, value.IsValidPhone());
    }

    [Theory]
    [InlineData("01001-000", true)]
    [InlineData("01001000", true)]
    [InlineData("1000", false)]
    public void IsValidCep_ShouldValidateExpectedCases(string value, bool expected)
    {
        Assert.Equal(expected, value.IsValidCep());
    }

    [Theory]
    [InlineData("https://rysecode.dev", true)]
    [InlineData("usuario123", true)]
    [InlineData("ab", false)]
    [InlineData("", false)]
    public void IsValidUrlOuUser_ShouldValidateExpectedCases(string value, bool expected)
    {
        Assert.Equal(expected, value.IsValidUrlOuUser());
    }

    [Theory]
    [InlineData("12345678", true)]
    [InlineData("abcdefghi", true)]
    [InlineData("1234567", false)]
    [InlineData("", false)]
    public void IsValidBasicPassword_ShouldValidateExpectedCases(string value, bool expected)
    {
        Assert.Equal(expected, value.IsValidBasicPassword());
    }
}

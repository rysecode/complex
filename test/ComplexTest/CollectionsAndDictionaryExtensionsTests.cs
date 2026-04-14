using Complex.Application.Common.Extensions;
using System.Collections;

namespace ComplexTest;

public class CollectionsAndDictionaryExtensionsTests
{
    [Fact]
    public void ToDictionary_ShouldGroupErrorsByAttributeName()
    {
        var errors = new List<string> { "required", "invalid" };

        var dictionary = errors.ToDictionary("email");

        Assert.Equal(2, dictionary["email"].Length);
        Assert.Contains("required", dictionary["email"]);
        Assert.Contains("invalid", dictionary["email"]);
    }

    [Fact]
    public void ToDictionaryObject_ShouldReturnObjectCompatiblePayload()
    {
        var errors = new List<string> { "invalid cpf" };

        var dictionary = errors.ToDictionaryObject("cpf");

        var values = Assert.IsType<string[]>(dictionary["cpf"]);
        Assert.Single(values);
        Assert.Equal("invalid cpf", values[0]);
    }

    [Fact]
    public void ToReadOnlyDictionary_FromHashtable_ShouldConvertEntries()
    {
        IDictionary source = new Hashtable
        {
            ["name"] = "complex",
            ["count"] = 2
        };

        var result = source.ToReadOnlyDictionary();

        Assert.Equal("complex", result["name"]);
        Assert.Equal(2, result["count"]);
    }

    [Fact]
    public void ToReadOnlyDictionary_FromGenericDictionaryWithNonStringKeys_ShouldConvertKeyToString()
    {
        IDictionary<int, bool> source = new Dictionary<int, bool>
        {
            [7] = true
        };

        var result = source.ToReadOnlyDictionary();

        Assert.True((bool)result["7"]!);
    }

    [Fact]
    public void AsReadOnly_ShouldReflectSourceValues()
    {
        IDictionary source = new Hashtable
        {
            ["status"] = "initial"
        };

        var readOnly = source.AsReadOnly();
        source["status"] = "updated";

        Assert.Equal("updated", readOnly["status"]);
    }

    [Fact]
    public void ToReadOnlyDictionary_WhenHashtableHasNonStringKeys_ShouldThrow()
    {
        IDictionary source = new Hashtable
        {
            [1] = "invalid"
        };

        Assert.Throws<InvalidCastException>(() => source.ToReadOnlyDictionary());
    }
}

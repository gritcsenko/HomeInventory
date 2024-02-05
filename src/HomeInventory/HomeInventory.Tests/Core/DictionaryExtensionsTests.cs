namespace HomeInventory.Tests.Core;

[UnitTest]
public class DictionaryExtensionsTests : BaseTest
{
    private readonly string _key;
    private readonly Ulid _value;
    private readonly string _invalidKey;
    private readonly IReadOnlyDictionary<string, Ulid> _roSut;

    public DictionaryExtensionsTests()
    {
        _key = Fixture.Create<string>();
        _value = Fixture.Create<Ulid>();
        _invalidKey = Fixture.Create<string>();
        var dictionary = new Dictionary<string, Ulid> { [_key] = _value };
        _roSut = dictionary;
    }

    [Fact]
    public void GetValueOptional2_Should_ReturnValue_WhenKeyIsPresent()
    {
        var result = DictionaryExtensions.GetValueOptional(_roSut, _key);

        result.Should().HaveValue(_value);
    }

    [Fact]
    public void GetValueOptional2_Should_ReturnDefaultValue_WhenKeyIsNotPresent()
    {
        var result = DictionaryExtensions.GetValueOptional(_roSut, _invalidKey);

        result.Should().HaveNoValue();
    }
}

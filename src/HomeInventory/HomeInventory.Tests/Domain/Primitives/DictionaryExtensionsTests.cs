using HomeInventory.Domain;

namespace HomeInventory.Tests.Domain.Primitives;

public class DictionaryExtensionsTests : BaseTest
{
    private readonly string _key;
    private readonly Guid _value;
    private readonly string _invalidKey;
    private readonly Guid _defaultValue;
    private readonly Dictionary<string, Guid> _sut;

    public DictionaryExtensionsTests()
    {
        _key = Fixture.Create<string>();
        _value = Fixture.Create<Guid>();
        _invalidKey = Fixture.Create<string>();
        _defaultValue = Fixture.Create<Guid>();
        _sut = new Dictionary<string, Guid>
        {
            [_key] = _value,
        };
    }

    [Fact]
    public void GetValueOrDefault_Should_ReturnValue_WhenKeyIsPresent()
    {
        var result = DictionaryExtensions.GetValueOrDefault(_sut, _key, _ => _defaultValue);

        result.Should().NotBe(_defaultValue);
        result.Should().Be(_value);
    }

    [Fact]
    public void GetValueOrDefault_Should_ReturnDefaultValue_WhenKeyIsNotPresent()
    {
        var result = DictionaryExtensions.GetValueOrDefault(_sut, _invalidKey, _ => _defaultValue);

        result.Should().NotBe(_value);
        result.Should().Be(_defaultValue);
    }

    [Fact]
    public void GetOrAdd_Should_ReturnValue_WhenKeyIsPresent()
    {
        var result = DictionaryExtensions.GetOrAdd(_sut, _key, _ => _defaultValue);

        result.Should().NotBe(_defaultValue);
        result.Should().Be(_value);
    }

    [Fact]
    public void GetOrAdd_Should_ReturnDefaultValue_WhenKeyIsNotPresent()
    {
        var result = DictionaryExtensions.GetOrAdd(_sut, _invalidKey, _ => _defaultValue);

        result.Should().NotBe(_value);
        result.Should().Be(_defaultValue);
        _sut.Should().Contain(KeyValuePair.Create(_invalidKey, _defaultValue));
    }
}

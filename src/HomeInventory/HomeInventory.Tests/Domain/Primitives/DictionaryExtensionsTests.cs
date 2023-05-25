using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Domain.Primitives;

[UnitTest]
public class DictionaryExtensionsTests : BaseTest
{
    private readonly string _key;
    private readonly Guid _value;
    private readonly string _invalidKey;
    private readonly Guid _defaultValue;
    private readonly IDictionary<string, Guid> _sut;
    private readonly IReadOnlyDictionary<string, Guid> _sut2;

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
        _sut2 = new Dictionary<string, Guid>
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
    public void GetValueOrDefault2_Should_ReturnValue_WhenKeyIsPresent()
    {
        var result = DictionaryExtensions.GetValueOrDefault(_sut2, _key, _ => _defaultValue);

        result.Should().NotBe(_defaultValue);
        result.Should().Be(_value);
    }

    [Fact]
    public void GetValueOrDefault2_Should_ReturnDefaultValue_WhenKeyIsNotPresent()
    {
        var result = DictionaryExtensions.GetValueOrDefault(_sut2, _invalidKey, _ => _defaultValue);

        result.Should().NotBe(_value);
        result.Should().Be(_defaultValue);
    }

    [Fact]
    public void GetValueOptional_Should_ReturnValue_WhenKeyIsPresent()
    {
        var result = DictionaryExtensions.GetValueOptional(_sut, _key);

        result.Should().HaveValue(_value);
    }

    [Fact]
    public void GetValueOptional_Should_ReturnDefaultValue_WhenKeyIsNotPresent()
    {
        var result = DictionaryExtensions.GetValueOptional(_sut, _invalidKey);

        result.Should().HaveNoValue();
    }

    [Fact]
    public void GetValueOptional2_Should_ReturnValue_WhenKeyIsPresent()
    {
        var result = DictionaryExtensions.GetValueOptional(_sut2, _key);

        result.Should().HaveValue(_value);
    }

    [Fact]
    public void GetValueOptional2_Should_ReturnDefaultValue_WhenKeyIsNotPresent()
    {
        var result = DictionaryExtensions.GetValueOptional(_sut2, _invalidKey);

        result.Should().HaveNoValue();
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

    [Fact]
    public void GetOrAdd2_Should_ReturnValue_WhenKeyIsPresent()
    {
        var result = DictionaryExtensions.GetOrAdd(_sut, _key, _defaultValue);

        result.Should().NotBe(_defaultValue);
        result.Should().Be(_value);
    }

    [Fact]
    public void GetOrAdd2_Should_ReturnDefaultValue_WhenKeyIsNotPresent()
    {
        var result = DictionaryExtensions.GetOrAdd(_sut, _invalidKey, _defaultValue);

        result.Should().NotBe(_value);
        result.Should().Be(_defaultValue);
        _sut.Should().Contain(KeyValuePair.Create(_invalidKey, _defaultValue));
    }
}

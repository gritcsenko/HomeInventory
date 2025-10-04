using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HomeInventory.Web.ErrorHandling;

namespace HomeInventory.Tests.Presentation.Web;

[UnitTest]
public sealed class DataContractJsonConverterTests() : BaseTest<DataContractJsonConverterTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void CanConvert_Should_ReturnTrue_When_TypeIsAssignableToT()
    {
        Given
            .Sut(out var sutVar);

        var then = When
            .Invoked(sutVar, static sut => sut.CanConvert(typeof(TestDataContract)));

        then
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void CanConvert_Should_ReturnFalse_When_TypeIsNotAssignableToT()
    {
        Given
            .Sut(out var sutVar);

        var then = When
            .Invoked(sutVar, static sut => sut.CanConvert(typeof(string)));

        then
            .Result(actual => actual.Should().BeFalse());
    }

    [Fact]
    public void Read_Should_ReturnDefault()
    {
        Given
            .Sut(out var sutVar)
            .New<string>(out var jsonVar, () => "{}");

        var then = When
            .Invoked(sutVar, jsonVar, static (sut, json) =>
            {
                var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
                reader.Read();
                var result = sut.Read(ref reader, typeof(TestDataContract), new());
                return result is null;
            });

        then
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void Write_Should_SerializeWithCamelCase()
    {
        Given
            .Sut(out var sutVar)
            .New<string>(out var propertyNameVar)
            .New<int>(out var numberValueVar)
            .New(out var testDataVar, propertyNameVar, numberValueVar, static (propName, numValue) => new TestDataContract
            {
                PropertyName = propName,
                NumberValue = numValue,
            })
            .WithOptions(sutVar, out var optionsVar);

        var then = When
            .Invoked(testDataVar, optionsVar, static (data, options) =>
                JsonSerializer.Serialize(data, options));

        then
            .Result(propertyNameVar, numberValueVar, static (actual, propName, numValue) =>
            {
                actual.Should().Contain(nameof(TestDataContract.PropertyName).ToCamelCase());
                actual.Should().Contain(nameof(TestDataContract.NumberValue).ToCamelCase());
                actual.Should().Contain(propName);
                actual.Should().Contain(numValue.ToString(CultureInfo.InvariantCulture));
            });
    }

    [Fact]
    public void Write_Should_SerializeWithIndentation()
    {
        Given
            .Sut(out var sutVar)
            .New<string>(out var propertyNameVar)
            .New<int>(out var numberValueVar)
            .New(out var testDataVar, propertyNameVar, numberValueVar, static (propName, numValue) => new TestDataContract
            {
                PropertyName = propName,
                NumberValue = numValue,
            })
            .WithOptions(sutVar, out var optionsVar);

        var then = When
            .Invoked(testDataVar, optionsVar, static (data, options) =>
                JsonSerializer.Serialize(data, options));

        then
            .Result(actual =>
            {
                actual.Should().Contain("\n");
                actual.Should().Contain("  ");
            });
    }

    [Fact]
    public void Write_Should_HandleNullValue()
    {
        Given
            .Sut(out var sutVar)
            .WithOptions(sutVar, out var optionsVar);

        var then = When
            .Invoked(optionsVar, static options =>
                JsonSerializer.Serialize<TestDataContract?>(null, options));

        then
            .Result(actual => actual.Should().Be("null"));
    }

    [Fact]
    public void Write_Should_SerializeComplexObject()
    {
        Given
            .Sut(out var sutVar)
            .New<string>(out var propertyNameVar)
            .New<int>(out var numberValueVar)
            .New<string>(out var nestedPropertyVar)
            .New(out var testDataVar, propertyNameVar, numberValueVar, nestedPropertyVar, static (propName, numValue, nestedProp) => new TestDataContract
            {
                PropertyName = propName,
                NumberValue = numValue,
                NestedData = new()
                {
                    NestedProperty = nestedProp,
                },
            })
            .WithOptions(sutVar, out var optionsVar);

        var then = When
            .Invoked(testDataVar, optionsVar, static (data, options) =>
                JsonSerializer.Serialize(data, options));

        then
            .Result(testDataVar, static (actual, data) =>
            {
                actual.Should().Contain(nameof(TestDataContract.PropertyName).ToCamelCase());
                actual.Should().Contain(data.PropertyName);
                actual.Should().Contain(nameof(TestDataContract.NumberValue).ToCamelCase());
                actual.Should().Contain(data.NumberValue.ToString(CultureInfo.InvariantCulture));
                actual.Should().Contain(nameof(TestDataContract.NestedData).ToCamelCase());
                actual.Should().Contain(nameof(NestedDataContract.NestedProperty).ToCamelCase());
                actual.Should().Contain(data.NestedData!.NestedProperty);
            });
    }
}

public sealed class DataContractJsonConverterTestsGivenContext(BaseTest test)
    : GivenContext<DataContractJsonConverterTestsGivenContext, JsonConverter<TestDataContract>>(test)
{
    protected override JsonConverter<TestDataContract> CreateSut() => new DataContractJsonConverter<TestDataContract>();

    public DataContractJsonConverterTestsGivenContext WithOptions(
        IVariable<JsonConverter<TestDataContract>> sutVar,
        out IVariable<JsonSerializerOptions> optionsVar) =>
        New(out optionsVar, sutVar, static sut => new()
        {
            Converters = { sut },
            WriteIndented = true,
        });
}

[DataContract]
public sealed class TestDataContract
{
    [DataMember]
    public string PropertyName { get; set; } = string.Empty;

    [DataMember]
    public int NumberValue { get; set; }

    [DataMember]
    public NestedDataContract? NestedData { get; set; }
}

[DataContract]
public sealed class NestedDataContract
{
    [DataMember]
    public string NestedProperty { get; set; } = string.Empty;
}


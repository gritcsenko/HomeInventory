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

    [Fact]
    public void Write_Should_SerializeLongNumber()
    {
        Given
            .New<long>(out var longValueVar)
            .New(out var testDataVar, longValueVar, static longValue => new TestDataContractWithLong
            {
                LongValue = longValue,
            })
            .New(out var converterVar, static () => new DataContractJsonConverter<TestDataContractWithLong>())
            .New(out var optionsVar, converterVar, static converter => new JsonSerializerOptions
            {
                Converters = { converter },
                WriteIndented = true,
            });

        var then = When
            .Invoked(testDataVar, optionsVar, static (data, options) =>
                JsonSerializer.Serialize(data, options));

        then
            .Result(testDataVar, static (actual, data) =>
            {
                actual.Should().Contain(nameof(TestDataContractWithLong.LongValue).ToCamelCase());
                actual.Should().Contain(data.LongValue.ToString(CultureInfo.InvariantCulture));
            });
    }

    [Fact]
    public void Write_Should_SerializeDoubleNumber()
    {
        Given
            .New<double>(out var doubleValueVar)
            .New(out var testDataVar, doubleValueVar, static doubleValue => new TestDataContractWithDouble
            {
                DoubleValue = doubleValue,
            })
            .New(out var converterVar, static () => new DataContractJsonConverter<TestDataContractWithDouble>())
            .New(out var optionsVar, converterVar, static converter => new JsonSerializerOptions
            {
                Converters = { converter },
                WriteIndented = true,
            });

        var then = When
            .Invoked(testDataVar, optionsVar, static (data, options) =>
                JsonSerializer.Serialize(data, options));

        then
            .Result(testDataVar, static (actual, data) =>
            {
                actual.Should().Contain(nameof(TestDataContractWithDouble.DoubleValue).ToCamelCase());
                actual.Should().Contain(data.DoubleValue.ToString(CultureInfo.InvariantCulture));
            });
    }

    [Fact]
    public void Write_Should_SerializeDecimalNumber()
    {
        Given
            .New<decimal>(out var decimalValueVar)
            .New(out var testDataVar, decimalValueVar, static decimalValue => new TestDataContractWithDecimal
            {
                DecimalValue = decimalValue,
            })
            .New(out var converterVar, static () => new DataContractJsonConverter<TestDataContractWithDecimal>())
            .New(out var optionsVar, converterVar, static converter => new JsonSerializerOptions
            {
                Converters = { converter },
                WriteIndented = true,
            });

        var then = When
            .Invoked(testDataVar, optionsVar, static (data, options) =>
                JsonSerializer.Serialize(data, options));

        then
            .Result(testDataVar, static (actual, data) =>
            {
                actual.Should().Contain(nameof(TestDataContractWithDecimal.DecimalValue).ToCamelCase());
                actual.Should().Contain(data.DecimalValue.ToString(CultureInfo.InvariantCulture));
            });
    }

    [Fact]
    public void Write_Should_SerializeArray()
    {
        Given
            .New<string>(out var item1Var)
            .New<string>(out var item2Var)
            .New(out var testDataVar, item1Var, item2Var, static (item1, item2) => new TestDataContractWithArray
            {
                Items = [item1, item2],
            })
            .New(out var converterVar, static () => new DataContractJsonConverter<TestDataContractWithArray>())
            .New(out var optionsVar, converterVar, static converter => new JsonSerializerOptions
            {
                Converters = { converter },
                WriteIndented = true,
            });

        var then = When
            .Invoked(testDataVar, optionsVar, static (data, options) =>
                JsonSerializer.Serialize(data, options));

        then
            .Result(testDataVar, static (actual, data) =>
            {
                actual.Should().Contain(nameof(TestDataContractWithArray.Items).ToCamelCase());
                actual.Should().Contain(data.Items[0]);
                actual.Should().Contain(data.Items[1]);
            });
    }

    [Fact]
    public void Write_Should_SerializeBooleanTrue()
    {
        Given
            .New(out var testDataVar, () => new TestDataContractWithBool
            {
                BoolValue = true,
            })
            .New(out var converterVar, static () => new DataContractJsonConverter<TestDataContractWithBool>())
            .New(out var optionsVar, converterVar, static converter => new JsonSerializerOptions
            {
                Converters = { converter },
                WriteIndented = true,
            });

        var then = When
            .Invoked(testDataVar, optionsVar, static (data, options) =>
                JsonSerializer.Serialize(data, options));

        then
            .Result(actual =>
            {
                actual.Should().Contain(nameof(TestDataContractWithBool.BoolValue).ToCamelCase());
                actual.Should().Contain("true");
            });
    }

    [Fact]
    public void Write_Should_SerializeBooleanFalse()
    {
        Given
            .New(out var testDataVar, () => new TestDataContractWithBool
            {
                BoolValue = false,
            })
            .New(out var converterVar, static () => new DataContractJsonConverter<TestDataContractWithBool>())
            .New(out var optionsVar, converterVar, static converter => new JsonSerializerOptions
            {
                Converters = { converter },
                WriteIndented = true,
            });

        var then = When
            .Invoked(testDataVar, optionsVar, static (data, options) =>
                JsonSerializer.Serialize(data, options));

        then
            .Result(actual =>
            {
                actual.Should().Contain(nameof(TestDataContractWithBool.BoolValue).ToCamelCase());
                actual.Should().Contain("false");
            });
    }

    [Fact]
    public void Write_Should_NotIndent_When_WriterAlreadyIndented()
    {
        Given
            .Sut(out var sutVar)
            .New<string>(out var propertyNameVar)
            .New(out var testDataVar, propertyNameVar, static propName => new TestDataContract
            {
                PropertyName = propName,
            });

        var then = When
            .Invoked(sutVar, testDataVar, static (sut, data) =>
            {
                using var memoryStream = new MemoryStream();
                using var writer = new Utf8JsonWriter(memoryStream, new() { Indented = true });

                sut.Write(writer, data, new() { WriteIndented = true });
                writer.Flush();

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            });

        then
            .Result(propertyNameVar, static (actual, propName) =>
            {
                actual.Should().Contain(nameof(TestDataContract.PropertyName).ToCamelCase());
                actual.Should().Contain(propName);
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
public sealed class TestDataContractWithLong
{
    [DataMember]
    public long LongValue { get; set; }
}

[DataContract]
public sealed class TestDataContractWithDouble
{
    [DataMember]
    public double DoubleValue { get; set; }
}

[DataContract]
public sealed class TestDataContractWithDecimal
{
    [DataMember]
    public decimal DecimalValue { get; set; }
}

[DataContract]
public sealed class TestDataContractWithArray
{
    [DataMember]
#pragma warning disable CA1819 // Properties should not return arrays
    public string[] Items { get; set; } = [];
#pragma warning restore CA1819
}

[DataContract]
public sealed class TestDataContractWithBool
{
    [DataMember]
    public bool BoolValue { get; set; }
}

[DataContract]
public sealed class NestedDataContract
{
    [DataMember]
    public string NestedProperty { get; set; } = string.Empty;
}

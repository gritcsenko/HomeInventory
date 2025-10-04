using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeInventory.Web.ErrorHandling;

internal static class JsonWriterOptions
{
    public static readonly System.Text.Json.JsonWriterOptions Indented = new() { Indented = true };
}

internal class DataContractJsonConverter<T> : JsonConverter<T>
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(T));

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => default;

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var rawJson = value.SerializeDataContract();
        using var jsonDocument = JsonDocument.Parse(rawJson);

        if (!options.WriteIndented || writer.Options.Indented)
        {
            writer.WriteElement(jsonDocument.RootElement);
            return;
        }

        using var memoryStream = new MemoryStream();
        using (var indentedWriter = new Utf8JsonWriter(memoryStream, JsonWriterOptions.Indented))
        {
            indentedWriter.WriteElement(jsonDocument.RootElement);
        }

        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var json = reader.ReadToEnd();
        writer.WriteRawValue(json);
    }
}

file static class Extensions
{
    public static void WriteElement(this Utf8JsonWriter writer, JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject())
                {
                    var camelCaseName = property.Name.ToCamelCase();
                    writer.WritePropertyName(camelCaseName);
                    WriteElement(writer, property.Value);
                }
                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                {
                    WriteElement(writer, item);
                }
                writer.WriteEndArray();
                break;

            case JsonValueKind.String:
                writer.WriteStringValue(element.GetString());
                break;

            case JsonValueKind.Number:
                if (element.TryGetInt32(out var intValue))
                {
                    writer.WriteNumberValue(intValue);
                }
                else if (element.TryGetInt64(out var longValue))
                {
                    writer.WriteNumberValue(longValue);
                }
                else if (element.TryGetDouble(out var doubleValue))
                {
                    writer.WriteNumberValue(doubleValue);
                }
                else
                {
                    writer.WriteNumberValue(element.GetDecimal());
                }
                break;

            case JsonValueKind.True:
                writer.WriteBooleanValue(true);
                break;

            case JsonValueKind.False:
                writer.WriteBooleanValue(false);
                break;

            case JsonValueKind.Null:
                writer.WriteNullValue();
                break;
            case JsonValueKind.Undefined:
            default:
                break;
        }
    }

    public static string SerializeDataContract<T>(this T value)
    {
        var serializer = new DataContractJsonSerializer(value?.GetType() ?? typeof(T));

        using var memoryStream = new MemoryStream();
        serializer.WriteObject(memoryStream, value);
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }    
}
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeInventory.Web.ErrorHandling;

internal class DataContractJsonConverter<T> : JsonConverter<T>
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(T));

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => default;

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var rawJson = SerializeDataContract(value);
        using var jsonDocument = JsonDocument.Parse(rawJson);
        JsonSerializer.Serialize(writer, jsonDocument.RootElement, _options);
    }

    private static string SerializeDataContract(T value)
    {
        var serializer = new DataContractJsonSerializer(value?.GetType() ?? typeof(T));

        using var memoryStream = new MemoryStream();
        serializer.WriteObject(memoryStream, value);
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
}
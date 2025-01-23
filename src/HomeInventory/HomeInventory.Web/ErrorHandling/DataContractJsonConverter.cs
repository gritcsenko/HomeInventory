using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeInventory.Web.ErrorHandling;

internal class DataContractJsonConverter<T> : JsonConverter<T>
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true, // Enable pretty-printing
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(T));

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => default;

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var serializer = new DataContractJsonSerializer(value?.GetType() ?? typeof(T));
        using var memoryStream = new MemoryStream();
        serializer.WriteObject(memoryStream, value);

        // Convert the raw JSON bytes into a string
        var rawJson = Encoding.UTF8.GetString(memoryStream.ToArray());

        // Parse the raw JSON into a JsonDocument for reformatting
        using var jsonDocument = JsonDocument.Parse(rawJson);
        // Write the formatted JSON using System.Text.Json
        JsonSerializer.Serialize(writer, jsonDocument.RootElement, _options);
    }
}
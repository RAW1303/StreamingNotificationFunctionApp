using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.JsonConverters;
public class Base64ObjectJsonConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var base64string = reader.GetString();

        if (base64string is null)
            return default;

        var json = Encoding.UTF8.GetString(reader.GetBytesFromBase64());
        return JsonSerializer.Deserialize<T>(json);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var json = JsonSerializer.Serialize(value);
        writer.WriteBase64StringValue(Encoding.UTF8.GetBytes(json));
    }
}

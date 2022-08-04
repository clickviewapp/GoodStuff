namespace ClickView.GoodStuff.IdGenerators.Serialization.SystemTextJson;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Abstractions;

/// <summary>
/// JsonConverter for <see cref="IdLong"/>
/// </summary>
public class IdLongConverter : JsonConverter<IdLong>
{
    /// <inheritdoc />
    public override IdLong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return IdLong.Parse(reader.GetString()!);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IdLong value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

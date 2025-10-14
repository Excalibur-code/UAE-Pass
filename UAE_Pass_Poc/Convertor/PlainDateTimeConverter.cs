using System.Text.Json;
using System.Text.Json.Serialization;

namespace UAE_Pass_Poc.Convertor
{
    public sealed class PlainDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd HH:mm:ss";
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.Parse(reader.GetString()!);
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(Format));

    }
}
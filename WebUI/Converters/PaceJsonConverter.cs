using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebUI.Models;

namespace WebUI.Converters;

public sealed class PaceJsonConverter : JsonConverter<Pace>
{
    public override Pace Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Accept: "m:ss" string, integer total seconds, or minutes as double
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (Pace.TryParse(s, CultureInfo.InvariantCulture, out var pace))
                return pace;

            throw new JsonException($"Invalid Pace string '{s}'. Expected mm:ss (e.g., \"4:59\").");
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt32(out var totalSeconds) && totalSeconds >= 0)
                return new Pace(totalSeconds);

            if (reader.TryGetDouble(out var minutes) && minutes >= 0 && double.IsFinite(minutes))
                return Pace.FromMinutesDouble(minutes);

            throw new JsonException("Invalid numeric Pace. Provide non-negative total seconds (int) or minutes (double).");
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (root.TryGetProperty("totalSeconds", out var tsProp) && tsProp.ValueKind == JsonValueKind.Number && tsProp.TryGetInt32(out var secs) && secs >= 0)
                return new Pace(secs);

            if (root.TryGetProperty("minutes", out var mProp) && mProp.ValueKind == JsonValueKind.Number)
            {
                var m = mProp.GetDouble();
                if (m >= 0 && double.IsFinite(m))
                    return Pace.FromMinutesDouble(m);
            }

            if (root.TryGetProperty("value", out var vProp) && vProp.ValueKind == JsonValueKind.String)
            {
                var s = vProp.GetString();
                if (Pace.TryParse(s, CultureInfo.InvariantCulture, out var pace))
                    return pace;
            }

            throw new JsonException("Invalid Pace object. Expected { totalSeconds:int } or { minutes:number } or { value:\"m:ss\" }.");
        }

        throw new JsonException($"Unexpected token {reader.TokenType} for Pace.");
    }

    public override void Write(Utf8JsonWriter writer, Pace value, JsonSerializerOptions options)
    {
        // Serialize as "m:ss" string
        writer.WriteStringValue(value.ToString(null, CultureInfo.InvariantCulture));
    }
}
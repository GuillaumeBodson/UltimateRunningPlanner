using GarminRunerz.Workout.Services.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebUI.Models;

namespace WebUI.Converters;

public class TrainingTemplateConverter : JsonConverter<TrainingTemplate>
{
    public override TrainingTemplate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject for TrainingTemplate.");

        using var doc = JsonDocument.ParseValue(ref reader);

        var plan = doc.Deserialize<Dictionary<DayOfWeek, RunType?>>();

        if (plan is null)
            return null;

        var builder = TrainingTemplate.CreateBuilder();
        foreach (var kvp in plan)
        {
            if (kvp.Value.HasValue)
                builder.With(kvp.Key, kvp.Value.Value);
        }

        return builder.Build();
    }

    public override void Write(Utf8JsonWriter writer, TrainingTemplate value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var day in Enum.GetValues<DayOfWeek>())
        {
            writer.WritePropertyName(day.ToString());
            if (value[day] is { } runType)
            {
                writer.WriteNumberValue((int)runType);
            }
            else
            {
                writer.WriteNullValue();
            }
        }

        writer.WriteEndObject();
    }
}

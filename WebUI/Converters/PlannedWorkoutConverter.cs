using System.Text.Json;
using System.Text.Json.Serialization;
using GarminRunerz.Workout.Services.Models;
using WebUI.Models;

namespace WebUI.Converters;

public sealed class PlannedWorkoutJsonConverter : JsonConverter<PlannedWorkout>
{
    public override PlannedWorkout? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("RunType", out var runTypeProp))
            throw new JsonException("Missing RunType discriminator.");

        RunType runType = (RunType)runTypeProp.GetInt32();

        Type concrete = runType switch
        {
            RunType.Easy or RunType.Recovery => typeof(EasyWorkout),
            RunType.Steady => typeof(SteadyWorkout),
            RunType.Intervals => typeof(IntervalWorkout),
            RunType.Tempo => typeof(TempoWorkout),
            RunType.LongRun => typeof(LongRunWorkout),
            RunType.Race => typeof(RaceWorkout),
            _ => typeof(DefaultWorkout)
        };

        var json = root.GetRawText();
        return (PlannedWorkout?)JsonSerializer.Deserialize(json, concrete, options);
    }

    public override void Write(Utf8JsonWriter writer, PlannedWorkout value, JsonSerializerOptions options)
    {
        // Serialize as its concrete runtime type
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;
using WebUI.Models;

namespace WebUI.Converters;

public class TrainingTemplateCollectionJsonConverter : JsonConverter<TrainingTemplateCollection>
{
    public override TrainingTemplateCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var list = JsonSerializer.Deserialize<IEnumerable<TrainingTemplate>>(ref reader, options);
        var collection = new TrainingTemplateCollection();
        if (list != null)
        {
            foreach (var item in list)
                collection.TryAdd(item);
        }
        return collection;
    }

    public override void Write(Utf8JsonWriter writer, TrainingTemplateCollection value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (IEnumerable<TrainingTemplate>)value, options);
    }
}

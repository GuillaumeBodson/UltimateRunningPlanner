using System.Collections;
using System.Text.Json.Serialization;
using WebUI.Converters;

namespace WebUI.Models;

[JsonConverter(typeof(TrainingTemplateCollectionJsonConverter))]
public class TrainingTemplateCollection : IEnumerable<TrainingTemplate>, IEnumerable
{
    private readonly Dictionary<int, TrainingTemplate> _templates = [];
    
    // Standard collection implementation omitted
    public int Count => _templates.Count;
    public IEnumerator<TrainingTemplate> GetEnumerator() => _templates.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _templates.Values.GetEnumerator();

    // Domain-specific methods
    public TrainingTemplate? GetForTrainingDays(int numberOfTrainingDays) =>
        _templates.TryGetValue(numberOfTrainingDays, out var template) ? template : null;

    public void Add(TrainingTemplate template) => _templates.TryAdd(template.TrainingDaysCount, template);
    public bool TryAdd(TrainingTemplate template) => _templates.TryAdd(template.TrainingDaysCount, template);
    public void Clear() => _templates.Clear();
    public bool Remove(int numberOfTrainingDays) => _templates.Remove(numberOfTrainingDays);

    // More selective factory method
    public static TrainingTemplateCollection CreateWithDefaults() => 
        [ 
            TrainingTemplate.Default4(), 
            TrainingTemplate.Default5() 
        ];
}

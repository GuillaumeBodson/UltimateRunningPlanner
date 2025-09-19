using GarminRunerz.Workout.Services.Models;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using WebUI.Converters;

namespace WebUI.Models;

/// <summary>
/// Weekly training template mapping days to planned RunType (null = rest).
/// Immutable value object with validation enforcing domain rules:
///  - At most one LongRun
///  - Quality workouts (Intervals, Tempo) limited to 1 if <=4 training days, else up to 2
/// </summary>
[JsonConverter(typeof(TrainingTemplateConverter))]
public sealed class TrainingTemplate
{
    private static readonly RunType[] QualityRunTypes = [ RunType.Intervals, RunType.Tempo ];
    private readonly IReadOnlyDictionary<DayOfWeek, RunType?> _plan;

    public RunType? this[DayOfWeek day] => _plan.TryGetValue(day, out var runType) ? runType : null;

    [JsonIgnore]
    public int TrainingDaysCount => _plan.Values.Count(v => v.HasValue);

    private TrainingTemplate(IDictionary<DayOfWeek, RunType?> plan)
    {
        // Ensure all 7 days present
        foreach (var day in Enum.GetValues<DayOfWeek>())
        {
            if (!plan.ContainsKey(day))
                plan[day] = null;
        }
        _plan = new ReadOnlyDictionary<DayOfWeek, RunType?>(new Dictionary<DayOfWeek, RunType?>(plan));
        Validate();
    }

    public static Builder CreateBuilder() => new();

    /// <summary>
    /// Returns a default 5-day template (Tue Easy, Wed Intervals, Fri Easy, Sat Steady, Sun LongRun).
    /// </summary>
    public static TrainingTemplate Default5()
        => CreateBuilder()
            .With(DayOfWeek.Tuesday, RunType.Easy)
            .With(DayOfWeek.Wednesday, RunType.Intervals | RunType.Tempo)
            .With(DayOfWeek.Friday, RunType.Easy)
            .With(DayOfWeek.Saturday, RunType.Steady)
            .With(DayOfWeek.Sunday, RunType.LongRun)
            .Build();

    public static TrainingTemplate Default4()
        => CreateBuilder()
            .With(DayOfWeek.Tuesday, RunType.Easy)
            .With(DayOfWeek.Thursday, RunType.Intervals | RunType.Tempo)
            .With(DayOfWeek.Saturday, RunType.Easy)
            .With(DayOfWeek.Sunday, RunType.LongRun)
            .Build();

    /// <summary>
    /// Schedule workouts for a given week based on the template.
    /// Tries to match RunType to template day; otherwise uses a rest day slot; finally any remaining day.
    /// Prioritizes LongRun, then quality, then others.
    /// </summary>
    public IEnumerable<(CustomWorkout workout, DayOfWeek day)> ScheduleWeek(IEnumerable<CustomWorkout> workouts)
    {
        ArgumentNullException.ThrowIfNull(workouts);
        var list = workouts.OrderBy(w => Priority(w.RunType)).ToList();

        var used = new HashSet<DayOfWeek>();
        var result = new List<(CustomWorkout, DayOfWeek)>();

        // Pre-index days by run type for fast lookup
        var byType = _plan
            .Where(p => p.Value.HasValue)
            .GroupBy(p => p.Value!.Value)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Key).ToList());

        foreach (var w in list)
        {
            DayOfWeek? chosen = null;

            // 1. Direct match
            if (byType.TryGetValue(w.RunType, out var candidates))
                chosen = candidates.FirstOrDefault(d => !used.Contains(d));

            // 2. If still none: try any rest day (null)
            chosen ??= _plan.Where(p => p.Value is null && !used.Contains(p.Key))
                              .Select(p => p.Key)
                              .Cast<DayOfWeek?>()
                              .FirstOrDefault();

            // 3. Fallback: any unused day
            chosen ??= _plan.Keys.FirstOrDefault(k => !used.Contains(k));

            if (chosen is null)
                throw new InvalidOperationException("Unable to assign a day for workout.");

            used.Add(chosen.Value);
            result.Add((w, chosen.Value));
        }

        return result;
    }

    private static int Priority(RunType type) =>
        type switch
        {
            RunType.LongRun => 0,
            RunType.Intervals => 1,
            RunType.Tempo => 2,
            _ => 5
        };

    private void Validate()
    {
        // Long run rule
        if (_plan.Values.Count(v => v == RunType.LongRun) > 1)
            throw new InvalidOperationException("Template cannot contain more than one LongRun.");

        // Quality rule
        var qualityCount = _plan.Values.Count(v => v is not null && QualityRunTypes.Contains(v.Value));
        var maxQuality = TrainingDaysCount <= 4 ? 1 : 2;
        if (qualityCount > maxQuality)
            throw new InvalidOperationException($"Template allows max {maxQuality} quality workouts (current: {qualityCount}).");
    }

    public sealed class Builder
    {
        private readonly Dictionary<DayOfWeek, RunType?> _plan = Enum.GetValues<DayOfWeek>()
            .ToDictionary(d => d, _ => (RunType?)null);

        public Builder With(DayOfWeek day, RunType runType)
        {
            _plan[day] = runType;
            return this;
        }

        public Builder Rest(DayOfWeek day)
        {
            _plan[day] = null;
            return this;
        }

        public TrainingTemplate Build() => new(_plan);
    }
}
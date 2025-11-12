using GarminRunerz.Workout.Services.Models;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using WebUI.Converters;
using WebUI.Services.Dtos;

namespace WebUI.Models;

/// <summary>
/// Weekly training template mapping days to an allowed RunType mask (null = rest).
/// Flags-aware: a stored value can combine multiple RunType flags to indicate acceptable workout categories
/// (e.g. Intervals | Tempo designates a generic quality session slot).
/// Domain rules enforced:
///  - At most one day whose mask includes LongRun
///  - Quality days (any mask intersecting Intervals or Tempo) limited to 1 if <=4 training days, else up to 2
/// Value object with structural equality (order: Mon..Sun).
/// </summary>
[JsonConverter(typeof(TrainingTemplateConverter))]
public sealed class TrainingTemplate : IEquatable<TrainingTemplate>
{
    private static readonly RunType[] QualityRunTypes = [ RunType.Intervals, RunType.Tempo ];

    private static readonly DayOfWeek[] OrderedDays =
    [
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
        DayOfWeek.Sunday
    ];

    private static readonly RunType QualityMask = RunType.Intervals | RunType.Tempo;

    private readonly IReadOnlyDictionary<DayOfWeek, RunType?> _plan;

    /// <summary>
    /// Returns the allowed RunType mask for the day (null = rest).
    /// </summary>
    public RunType? this[DayOfWeek day] => _plan.TryGetValue(day, out var runType) ? runType : null;

    [JsonIgnore]
    public int TrainingDaysCount => _plan.Values.Count(v => v.HasValue);

    private TrainingTemplate(IDictionary<DayOfWeek, RunType?> plan)
    {
        // Ensure all 7 days present
        foreach (var day in Enum.GetValues<DayOfWeek>())
        {
            if (!plan.ContainsKey(day)) plan[day] = null;
        }

        // Normalize: treat zero-mask as rest
        foreach (var kvp in plan.Where(k => k.Value.HasValue && k.Value!.Value == 0))
        {
            plan[kvp.Key] = null;
        }

        _plan = new ReadOnlyDictionary<DayOfWeek, RunType?>(new Dictionary<DayOfWeek, RunType?>(plan));
        Validate();
    }

    public static Builder CreateBuilder() => new();

    public static TrainingTemplate Default5()
        => CreateBuilder()
            .WithAllowed(DayOfWeek.Tuesday, RunType.Easy)
            .WithAllowed(DayOfWeek.Wednesday, RunType.Intervals | RunType.Tempo) // quality slot
            .WithAllowed(DayOfWeek.Friday, RunType.Easy)
            .WithAllowed(DayOfWeek.Saturday, RunType.Steady)
            .WithAllowed(DayOfWeek.Sunday, RunType.LongRun)
            .Build();

    public static TrainingTemplate Default4()
        => CreateBuilder()
            .WithAllowed(DayOfWeek.Tuesday, RunType.Easy)
            .WithAllowed(DayOfWeek.Thursday, RunType.Intervals | RunType.Tempo)
            .WithAllowed(DayOfWeek.Saturday, RunType.Easy)
            .WithAllowed(DayOfWeek.Sunday, RunType.LongRun)
            .Build();

    /// <summary>
    /// Schedule workouts into days honoring allowed masks:
    /// 1. Day whose mask & workout.RunType != 0 (match)
    /// 2. Rest day (null)
    /// 3. Any remaining unused day
    /// LongRun / quality workouts prioritized via Priority().
    /// </summary>
    public IEnumerable<(WorkoutDto workout, DayOfWeek day)> ScheduleWeek(IEnumerable<WorkoutDto> workouts)
    {
        ArgumentNullException.ThrowIfNull(workouts);
        var ordered = workouts.OrderBy(w => Priority(w.RunType)).ToList();
        var used = new HashSet<DayOfWeek>();
        var result = new List<(WorkoutDto, DayOfWeek)>();

        foreach (var w in ordered)
        {
            DayOfWeek? chosen =
                // 1. Mask match
                _plan.Where(p => p.Value is { } m && (m & w.RunType) != 0 && !used.Contains(p.Key))
                     .Select(p => (DayOfWeek?)p.Key)
                     .FirstOrDefault()
                // 2. Rest day
                ?? _plan.Where(p => p.Value is null && !used.Contains(p.Key))
                        .Select(p => (DayOfWeek?)p.Key)
                        .FirstOrDefault()
                // 3. Any unused day
                ?? _plan.Keys.FirstOrDefault(k => !used.Contains(k));

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
        // LongRun rule: any mask containing LongRun
        if (_plan.Values.Count(v => v is { } m && (m & RunType.LongRun) != 0) > 1)
            throw new InvalidOperationException("Template cannot contain more than one LongRun day.");

        // Quality day count: days whose mask intersects quality mask
        var qualityDays = _plan.Values.Count(v => v is { } m && (m & QualityMask) != 0);
        var maxQuality = TrainingDaysCount <= 4 ? 1 : 2;
        if (qualityDays > maxQuality)
            throw new InvalidOperationException($"Template allows max {maxQuality} quality workout days (current: {qualityDays}).");
    }

    // Value semantics (structural Monday->Sunday)
    public bool Equals(TrainingTemplate? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        foreach (var d in OrderedDays)
        {
            if (this[d] != other[d]) return false;
        }
        return true;
    }

    public override bool Equals(object? obj) => Equals(obj as TrainingTemplate);

    public static bool operator ==(TrainingTemplate? left, TrainingTemplate? right)
        => ReferenceEquals(left, right) || (left is not null && left.Equals(right));

    public static bool operator !=(TrainingTemplate? left, TrainingTemplate? right) => !(left == right);

    public override int GetHashCode()
    {
        var hc = new HashCode();
        foreach (var d in OrderedDays)
        {
            var v = this[d];
            // Use -1 sentinel for rest to distinguish from mask=0 normalization (we already convert 0 → rest)
            hc.Add(v.HasValue ? (int)v.Value : -1);
        }
        return hc.ToHashCode();
    }

    public sealed class Builder
    {
        private readonly Dictionary<DayOfWeek, RunType?> _plan = Enum.GetValues<DayOfWeek>()
            .ToDictionary(d => d, _ => (RunType?)null);

        /// <summary>
        /// Assign an allowed run type mask to a day (null/empty mask = rest).
        /// </summary>
        public Builder WithAllowed(DayOfWeek day, RunType allowedMask)
        {
            _plan[day] = allowedMask == 0 ? null : allowedMask;
            return this;
        }

        /// <summary>
        /// Backwards-compatible single value (can still be a mask).
        /// </summary>
        public Builder With(DayOfWeek day, RunType runType) => WithAllowed(day, runType);

        public Builder Rest(DayOfWeek day)
        {
            _plan[day] = null;
            return this;
        }

        public TrainingTemplate Build() => new(_plan);
    }
}
using GarminRunerz.Workout.Services.Models;

namespace WebUI.Models;

public record AthletePreferences
{
    public int WorkoutPerWeek { get; set; } = 4;

    public Dictionary<RunType, IWorkoutPreferences?> WorkoutPreferences { get; set; } =
     WorkoutPreferenceFactory.CreateDefaults();

    public IWorkoutPreferences GetPreferencesForRunType(RunType runType)
    {
        if (WorkoutPreferences.TryGetValue(runType, out var prefs) && prefs is not null)
        {
            return prefs;
        }
        return WorkoutPreferences[RunType.Other]!;
    }
}

public interface IWorkoutPreferences
{
    TimeSpan? WarmUpDuration { get; set; }
    TimeSpan? CoolDownDuration { get; set; }
    bool IncludeWarmUpCoolDown { get; }
}

public sealed class WorkoutPreferences : IWorkoutPreferences
{
    private static readonly TimeSpan DefaultWarmUp = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan DefaultCoolDown = TimeSpan.FromMinutes(10);

    private TimeSpan _warmUpDuration = DefaultWarmUp;
    private TimeSpan _coolDownDuration = DefaultCoolDown;


    public bool IncludeWarmUpCoolDown => WarmUpDuration.HasValue || CoolDownDuration.HasValue;
    public TimeSpan? WarmUpDuration
    {
        get => _warmUpDuration;
        set
        {
            if (value.HasValue)
                _warmUpDuration = NormalizeNonNegative(value.Value);
        }
    }

    public TimeSpan? CoolDownDuration
    {
        get => _coolDownDuration;
        set
        {
            if (value.HasValue)
                _coolDownDuration = NormalizeNonNegative(value.Value);
        }
    }

    public WorkoutPreferences(
        TimeSpan? warmUpDuration,
        TimeSpan? coolDownDuration)
    {
        if (warmUpDuration.HasValue)
            _warmUpDuration = NormalizeNonNegative(warmUpDuration.Value);
        if (coolDownDuration.HasValue)
            _coolDownDuration = NormalizeNonNegative(coolDownDuration.Value);
    }

    private static TimeSpan NormalizeNonNegative(TimeSpan value) =>
        value < TimeSpan.Zero ? TimeSpan.Zero : value;
}

public static class WorkoutPreferenceFactory
{
    public static Dictionary<RunType, IWorkoutPreferences?> CreateDefaults() => new()
    {
        { RunType.Easy,      new WorkoutPreferences(null, null) },
        { RunType.LongRun,   new WorkoutPreferences(null, null) },
        { RunType.Tempo,     new WorkoutPreferences(TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10)) },
        { RunType.Intervals, new WorkoutPreferences(TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10)) },
        { RunType.Recovery,  null },
        { RunType.Steady,    null },
        { RunType.Other,     new WorkoutPreferences(null, null) },
    };    
}
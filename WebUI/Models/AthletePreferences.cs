using GarminRunerz.Workout.Services.Models;

namespace WebUI.Models;

public record AthletePreferences
{
    public int WorkoutPerWeek { get; set; } = 4;
    public bool IsDarkMode { get; set; } = true;
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
    private TimeSpan? _warmUpDuration;
    private TimeSpan? _coolDownDuration;


    public bool IncludeWarmUpCoolDown => WarmUpDuration > TimeSpan.Zero|| CoolDownDuration > TimeSpan.Zero;
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

    // for json serialization
    public WorkoutPreferences()
    {
        
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
        { RunType.Easy,      null },
        { RunType.LongRun,   new WorkoutPreferences(null, null) },
        { RunType.Tempo,     new WorkoutPreferences(TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10)) },
        { RunType.Intervals, new WorkoutPreferences(TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10)) },
        { RunType.Recovery,  null },
        { RunType.Steady,    null },
        { RunType.Other,     new WorkoutPreferences(null, null) },
    };    
}
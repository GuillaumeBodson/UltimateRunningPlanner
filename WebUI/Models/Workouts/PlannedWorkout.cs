using GarminRunerz.Workout.Services.Models;
using MudBlazor;
using System.Globalization;
using System.MudPlanner;
using System.Text.Json.Serialization;
using WebUI.Converters;

namespace WebUI.Models.Workouts;

[JsonConverter(typeof(PlannedWorkoutJsonConverter))]
public abstract class PlannedWorkout
{
    public int Id { get; set; }
    public int WeekNumber { get; set; }
    public RunType RunType { get; set; }
    public int TotalDuration { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public int EstimatedDistance { get; set; }
    public int EstimatedTime { get; set; }
    public Pace Pace { get; set; }

    /// <summary>
    /// Display name (run-type specific).
    /// </summary>
    public abstract string Name { get; }

    protected abstract Color CalendarColor { get; }
    protected abstract string EventLabel { get; }

    // New estimation methods
    /// <summary>
    /// Calculates the estimated distance for this workout based on an athlete's paces.
    /// </summary>
    public int CalculateEstimatedDistance(Athlete athlete)
    {
        ArgumentNullException.ThrowIfNull(athlete);

        // Get speeds based on athlete data
        var easySpeed = athlete.EasyPace.ToMeterPerSeconds();
        int warmUpCoolDownDuration = CalculateWarmUpCoolDownDuration(athlete.AthletePreferences);

        // Calculate distances based on workout structure
        double distance = CalculateWorkoutDistanceCore(easySpeed) + (warmUpCoolDownDuration * (double)easySpeed);
        
        // Round to nearest 100m (consistent with original)
        return (int)Math.Ceiling(distance / 100) * 100;
    }
    
    /// <summary>
    /// Calculates the estimated duration for this workout.
    /// </summary>
    public int CalculateEstimatedDuration(AthletePreferences preferences)
    {
        ArgumentNullException.ThrowIfNull(preferences);

        int warmUpCoolDownDuration = CalculateWarmUpCoolDownDuration(preferences);        

        // Allow subclasses to add workout-specific duration
        int workoutSpecificDuration = CalculateWorkoutDurationCore();
        
        return workoutSpecificDuration + warmUpCoolDownDuration;
    }

    protected virtual int CalculateWarmUpCoolDownDuration(AthletePreferences preferences)
    {
        var workoutPrefs = preferences.GetPreferencesForRunType(RunType);

        int warmUpDuration = (int)(workoutPrefs.WarmUpDuration?.TotalSeconds ?? 0);
        int coolDownDuration = (int)(workoutPrefs.CoolDownDuration?.TotalSeconds ?? 0);

        return warmUpDuration + coolDownDuration;
    }


    /// <summary>
    /// Core distance calculation logic. Override in subclasses for specific workout types.
    /// </summary>
    protected virtual double CalculateWorkoutDistanceCore(decimal easySpeed)
    {
        // Base implementation for continuous runs
        return TotalDuration * (double)easySpeed;
    }
    
    /// <summary>
    /// Core duration calculation (excluding warm-up/cool-down). Override in subclasses.
    /// </summary>
    protected virtual int CalculateWorkoutDurationCore()
    {
        // Base case: just the total duration
        return TotalDuration;
    }
    public CalendarEvent ToCalendarEvent()
    {
        return new CalendarEvent(Id, CalendarEventName(), Description, CalendarColor, Date);
    }

    private string CalendarEventName()
    {
        return $"W{WeekNumber} {EventLabel} {Math.Round(EstimatedDistance / 1000.0, 1)}km";
    }

    protected string FormatPace()
    {
        return Pace.ToMinutesDouble().ToString("0.##", CultureInfo.InvariantCulture);
    }
}

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
        
        // Allow subclasses to provide custom effort speed
        var effortSpeed = GetEffortSpeed(athlete);
        
        // Calculate distances based on workout structure
        double distance = CalculateWorkoutDistanceCore(easySpeed, effortSpeed);
        
        // Round to nearest 100m (consistent with original)
        return (int)Math.Ceiling(distance / 100) * 100;
    }
    
    /// <summary>
    /// Calculates the estimated duration for this workout.
    /// </summary>
    public int CalculateEstimatedDuration()
    {
        // Base implementation for continuous runs
        int warmUpDuration = 15 * 60; // 15 minutes in seconds
        int coolDownDuration = 10 * 60; // 10 minutes in seconds
        
        // Allow subclasses to add workout-specific duration
        int workoutSpecificDuration = CalculateWorkoutDurationCore();
        
        return warmUpDuration + workoutSpecificDuration + coolDownDuration;
    }
    
    // Template methods - default implementations
    /// <summary>
    /// Calculates the speed used for effort segments based on the athlete's profile.
    /// </summary>
    protected virtual decimal GetEffortSpeed(Athlete athlete)
    {
        // Default to the workout's own pace
        return athlete.EasyPace.ToMeterPerSeconds();
    }
    
    /// <summary>
    /// Core distance calculation logic. Override in subclasses for specific workout types.
    /// </summary>
    protected virtual double CalculateWorkoutDistanceCore(decimal easySpeed, decimal effortSpeed)
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

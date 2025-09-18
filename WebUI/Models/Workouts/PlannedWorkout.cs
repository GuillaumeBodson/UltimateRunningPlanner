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
    //public int Repetitions { get; set; }
    //public double RunDuration { get; set; }
    //public double CoolDownDuration { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public int EstimatedDistance { get; set; }
    public int EstimatedTime { get; set; }
    public Pace Pace { get; set; }

    /// <summary>
    /// Display name (run-type specific).
    /// </summary>
    public abstract string Name { get; }

    public CalendarEvent ToCalendarEvent()
    {
        return new CalendarEvent(Id, CalendarEventName(), Description, CalendarColor, Date);
    }

    protected abstract Color CalendarColor { get; }
    protected abstract string EventLabel { get; }

    private string CalendarEventName()
    {
        return $"W{WeekNumber} {EventLabel} {Math.Round(EstimatedDistance / 1000.0, 1)}km";
    }

    protected string FormatPace()
    {
        return Pace.ToMinutesDouble().ToString("0.##", CultureInfo.InvariantCulture);
    }
}

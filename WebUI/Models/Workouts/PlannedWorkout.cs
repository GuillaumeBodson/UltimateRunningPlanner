using GarminRunerz.Workout.Services.Models;
using MudBlazor;
using System;
using System.Globalization;
using System.MudPlanner;
using System.Text.Json.Serialization;
using WebUI.Converters;

namespace WebUI.Models.Workouts;

[JsonConverter(typeof(PlannedWorkoutJsonConverter))]
public abstract class PlannedWorkout : CustomWorkout
{
    public DateOnly Date { get; set; }
    public int EstimatedDistance { get; set; }
    public int EstimatedTime { get; set; }
    public new Pace Pace { get; set; }

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

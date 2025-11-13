using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class IntervalWorkout : StructuredWorkout
{
    public override string Name => $"W{WeekNumber} {StringifyDetails()}";
    protected override Color CalendarColor => Color.Warning;
    protected override string EventLabel => "Intervals";
}
using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class TempoWorkout : StructuredWorkout
{
    public override string Name => $"W{WeekNumber} {StringifyDetails()}";

    public override Color CalendarColor => Color.Warning;
    public override string EventLabel => "Tempo";
}
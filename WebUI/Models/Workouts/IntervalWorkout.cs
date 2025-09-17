using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class IntervalWorkout : PlannedWorkout
{
    public override string Name => $"W{WeekNumber} {Repetitions}x{RunDuration}\" @ {FormatPace()}min/km";

    protected override Color CalendarColor => Color.Warning;
    protected override string EventLabel => "Intervals";
}
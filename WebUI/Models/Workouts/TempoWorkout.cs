using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class TempoWorkout : PlannedWorkout, IQualityWorkout
{
    public int Repetitions { get; set; }
    public double RunDuration { get; set; }
    public double CoolDownDuration { get; set; }
    public override string Name => Repetitions > 0
        ? $"W{WeekNumber} {Repetitions}x{RunDuration}\" @ {FormatPace()}min/km"
        : $"W{WeekNumber} Tempo run ({Math.Round(EstimatedDistance / 1000.0, 1)}km)";

    protected override Color CalendarColor => Color.Warning;
    protected override string EventLabel => "Tempo";
}
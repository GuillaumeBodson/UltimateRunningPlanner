using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class RaceWorkout : PlannedWorkout
{
    public override string Name => "Race day!";

    protected override Color CalendarColor => Color.Error;
    protected override string EventLabel => "Race";

    protected override double CalculateWorkoutDistanceCore(decimal easySpeed)
        => TotalDuration * (double)Pace.ToMeterPerSeconds();
}
using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class RaceWorkout : PlannedWorkout
{
    public override string Name => "Race day!";

    public override Color CalendarColor => Color.Error;
    public override string EventLabel => "Race";

    protected override double CalculateWorkoutDistanceCore(decimal easySpeed)
        => TotalDuration * (double)Pace.ToMeterPerSeconds();
}
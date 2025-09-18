using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class SteadyWorkout : PlannedWorkout, IQualityWorkout
{
    public int Repetitions { get; set; }
    public double RunDuration { get; set; }
    public double CoolDownDuration { get; set; }

    public override string Name
    {
        get
        {
            double km = EstimatedDistance / 1000.0;
            if (Repetitions == 0)
                return $"W{WeekNumber} Steady run ({km}km)";

            return $"W{WeekNumber} Steady run with {RunDuration}\" @ {FormatPace()}min/km";
        }
    }

    protected override Color CalendarColor => Color.Info;
    protected override string EventLabel => "Steady";
}
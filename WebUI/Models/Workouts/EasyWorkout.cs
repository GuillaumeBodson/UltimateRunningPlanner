using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class EasyWorkout : PlannedWorkout
{
    public override string Name
    {
        get
        {
            double km = EstimatedDistance / 1000.0;
            
            return $"W{WeekNumber} Easy run ({km}km)";
        }
    }

    protected override Color CalendarColor => Color.Info;
    protected override string EventLabel => "Easy";
}
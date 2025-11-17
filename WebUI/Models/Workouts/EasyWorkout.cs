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

    public override Color CalendarColor => Color.Info;
    public override string EventLabel => "Easy";
}
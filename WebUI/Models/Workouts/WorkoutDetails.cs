using GarminRunerz.Workout.Services.Models;

namespace WebUI.Models.Workouts;

public class WorkoutDetails
{
    public int Repetitions { get; set; }
    public int EffortDuration { get; set; }
    public int RecoveryDuration { get; set; }
    public PaceType PaceType { get; set; }
    public Pace Pace { get; set; }
    public WorkoutDetails? Details
    {
        get
        {
            try
            {
                return DetailsCollection?.SingleOrDefault();
            }
            catch(InvalidOperationException)
            {
                return null;
            }
        }
    }

    public List<WorkoutDetails>? DetailsCollection { get; set; } = null;
}

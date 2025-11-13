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
            if (DetailsCollection == null || DetailsCollection.Count != 1)
                return null;

            return DetailsCollection.SingleOrDefault();
        }
    }

    public List<WorkoutDetails>? DetailsCollection { get; set; } = null;
}

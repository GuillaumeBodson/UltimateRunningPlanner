
using GarminRunerz.Workout.Services.Models;

namespace WebUI.Models.Workouts;

public interface IStructuredWorkout
{
    bool IsEmpty { get; }
    WorkoutDetails? Details { get; }
    List<WorkoutDetails>? DetailsCollection { get; set; }

    int GetEffortDistance();
    int GetDetailsDuration();
    TimeSpan WarmUp { get; set; }
    TimeSpan CoolDown { get; set; }

    public RunType RunType { get; }
}

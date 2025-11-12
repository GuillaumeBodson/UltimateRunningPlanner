using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Models.Workouts;

namespace WebUI.Services.Interfaces;

public interface IWorkoutMetadataFactory
{
    WorkoutMetadata Create(PlannedWorkout plannedWorkout, Athlete athlete, IWorkoutPreferences prefs);
}

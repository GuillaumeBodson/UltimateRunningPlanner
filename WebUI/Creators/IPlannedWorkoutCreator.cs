using GarminRunerz.Workout.Services.Models;
using WebUI.Models;

namespace WebUI.Services;

/// <summary>
/// Abstract factory for a specific RunType family — responsible for creating a concrete PlannedWorkout.
/// </summary>
public interface IPlannedWorkoutCreator
{
    bool CanCreate(RunType runType);
    PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date);
}
using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Models.Workouts;

namespace WebUI.Services.Interfaces;

/// <summary>
/// Application-level factory used by callers to obtain a typed PlannedWorkout.
/// </summary>
public interface IPlannedWorkoutFactory
{
    PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date);
}
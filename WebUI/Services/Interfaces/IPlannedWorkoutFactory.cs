using GarminRunerz.Workout.Services.Models;
using WebUI.Models;

namespace WebUI.Services.Interfaces;

/// <summary>
/// Application-level factory used by callers to obtain a typed PlannedWorkout.
/// </summary>
public interface IPlannedWorkoutFactory
{
    PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date);
}
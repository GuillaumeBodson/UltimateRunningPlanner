using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Models.Workouts;
using WebUI.Services.Dtos;

namespace WebUI.Services.Interfaces;

/// <summary>
/// Application-level factory used by callers to obtain a typed PlannedWorkout.
/// </summary>
public interface IPlannedWorkoutFactory
{
    PlannedWorkout Create(WorkoutDto workout, Athlete athlete, DateOnly date);
}
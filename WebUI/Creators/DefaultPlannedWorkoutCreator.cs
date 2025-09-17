using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Models.Workouts;
using WebUI.Services;

namespace WebUI.Creators;

public class DefaultPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    // Fallback for Other and unknown
    public override bool CanCreate(RunType runType) => runType == RunType.Other;

    protected override PlannedWorkout CreateInstance() => new DefaultWorkout();
}
using GarminRunerz.Workout.Services.Models;
using WebUI.Models;

namespace WebUI.Services.Creators;

public class DefaultPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    // Fallback for Other and unknown
    public override bool CanCreate(RunType runType) => runType == RunType.Other;

    protected override PlannedWorkout CreateInstance() => new DefaultWorkout();
}
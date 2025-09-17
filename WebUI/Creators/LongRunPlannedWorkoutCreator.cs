using GarminRunerz.Workout.Services.Models;
using WebUI.Models;

namespace WebUI.Services.Creators;

public class LongRunPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.LongRun;

    protected override PlannedWorkout CreateInstance() => new LongRunWorkout();
}
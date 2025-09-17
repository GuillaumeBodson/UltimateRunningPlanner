using GarminRunerz.Workout.Services.Models;
using WebUI.Models;

namespace WebUI.Services.Creators;

public class EasyPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Easy || runType == RunType.Recovery;

    protected override PlannedWorkout CreateInstance() => new EasyWorkout();
}
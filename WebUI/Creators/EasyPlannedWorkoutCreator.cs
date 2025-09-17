using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Models.Workouts;
using WebUI.Services;

namespace WebUI.Creators;

public class EasyPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Easy || runType == RunType.Recovery;

    protected override PlannedWorkout CreateInstance() => new EasyWorkout();
}
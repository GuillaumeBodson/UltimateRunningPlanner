using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Models.Workouts;
using WebUI.Services;

namespace WebUI.Creators;

public class LongRunPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.LongRun;

    protected override PlannedWorkout CreateInstance() => new LongRunWorkout();
}
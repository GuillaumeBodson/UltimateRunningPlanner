using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Models.Workouts;
using WebUI.Services;

namespace WebUI.Creators;

public class SteadyPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Steady;

    protected override PlannedWorkout CreateInstance() => new SteadyWorkout();
}
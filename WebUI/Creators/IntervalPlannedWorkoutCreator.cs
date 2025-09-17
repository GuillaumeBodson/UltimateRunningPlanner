using GarminRunerz.Workout.Services.Models;
using WebUI.Models;

namespace WebUI.Services.Creators;

public class IntervalPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Intervals;

    protected override PlannedWorkout CreateInstance() => new IntervalWorkout();
}
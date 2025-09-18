using GarminRunerz.Workout.Services.Models;
using WebUI.Models.Workouts;
using WebUI.Services;

namespace WebUI.Creators;

public class IntervalPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Intervals;

    protected override PlannedWorkout CreateInstance() => new IntervalWorkout();

}
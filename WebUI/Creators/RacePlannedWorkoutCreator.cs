using GarminRunerz.Workout.Services.Models;
using WebUI.Models;

namespace WebUI.Services.Creators;

public class RacePlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Race;

    protected override PlannedWorkout CreateInstance() => new RaceWorkout();
}
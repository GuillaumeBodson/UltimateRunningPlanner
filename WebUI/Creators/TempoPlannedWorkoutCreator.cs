using GarminRunerz.Workout.Services.Models;
using WebUI.Models;

namespace WebUI.Services.Creators;

public class TempoPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Tempo;

    protected override PlannedWorkout CreateInstance() => new TempoWorkout();
}
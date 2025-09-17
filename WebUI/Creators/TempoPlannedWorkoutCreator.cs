using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Models.Workouts;
using WebUI.Services;

namespace WebUI.Creators;

public class TempoPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Tempo;

    protected override PlannedWorkout CreateInstance() => new TempoWorkout();
}
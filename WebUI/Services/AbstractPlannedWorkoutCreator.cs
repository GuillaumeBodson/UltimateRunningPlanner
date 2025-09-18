using GarminRunerz.Workout.Services.Models;
using WebUI.Creators;
using WebUI.Models;
using WebUI.Models.Workouts;

namespace WebUI.Services;

/// <summary>
/// Shared helper for concrete creators: provides common population/estimation logic.
/// </summary>
public abstract class AbstractPlannedWorkoutCreator : IPlannedWorkoutCreator
{
    public abstract bool CanCreate(RunType runType);

    public PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date)
    {
        ArgumentNullException.ThrowIfNull(workout);
        ArgumentNullException.ThrowIfNull(athlete);

        var instance = CreateInstance();
        PopulateCommon(instance, workout, athlete, date);
        return instance;
    }

    protected abstract PlannedWorkout CreateInstance();

    protected virtual void PopulateCommon(PlannedWorkout instance, CustomWorkout workout, Athlete athlete, DateOnly date)
    {
        instance.Id = workout.Id;
        instance.WeekNumber = workout.WeekNumber;
        instance.RunType = workout.RunType;
        instance.TotalDuration = workout.TotalDuration;
        instance.Pace = Pace.FromMinutesDecimal(workout.Pace);
        instance.Description = workout.Description;
        instance.Date = date;
        instance.EstimatedDistance = WorkoutEstimator.EstimateDistance(workout, athlete);
        instance.EstimatedTime = WorkoutEstimator.EstimateDuration(workout);

        if (instance is IStructuredWorkout qualityWorkout)
        {
            qualityWorkout.Repetitions = workout.Repetitions;
            qualityWorkout.IntervalDuration = (int)Math.Round(workout.RunDuration);
            qualityWorkout.RecoveryDuration = (int)Math.Round(workout.CoolDownDuration);
        }
    }
}
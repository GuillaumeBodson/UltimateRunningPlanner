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
        if (workout is null) throw new ArgumentNullException(nameof(workout));
        if (athlete is null) throw new ArgumentNullException(nameof(athlete));

        var instance = CreateInstance();
        PopulateCommon(instance, workout, athlete);
        instance.Date = date;
        return instance;
    }

    protected abstract PlannedWorkout CreateInstance();

    protected void PopulateCommon(PlannedWorkout instance, CustomWorkout workout, Athlete athlete)
    {
        instance.Id = workout.Id;
        instance.WeekNumber = workout.WeekNumber;
        instance.RunType = workout.RunType;
        instance.TotalDuration = workout.TotalDuration;
        instance.Repetitions = workout.Repetitions;
        instance.RunDuration = workout.RunDuration;
        instance.CoolDownDuration = workout.CoolDownDuration;
        instance.Pace = Pace.FromMinutesDecimal(workout.Pace);
        instance.Speed = workout.Speed;
        instance.Description = workout.Description;

        instance.EstimatedDistance = WorkoutEstimator.EstimateDistance(workout, athlete);
        instance.EstimatedTime = WorkoutEstimator.EstimateDuration(workout);
    }
}
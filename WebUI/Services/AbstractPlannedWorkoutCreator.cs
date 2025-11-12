using GarminRunerz.Workout.Services.Models;
using WebUI.Creators;
using WebUI.Mappers;
using WebUI.Models;
using WebUI.Models.Workouts;
using WebUI.Services.Dtos;

namespace WebUI.Services;

/// <summary>
/// Shared helper for concrete creators: provides common population/estimation logic.
/// </summary>
public abstract class AbstractPlannedWorkoutCreator : IPlannedWorkoutCreator
{
    public abstract bool CanCreate(RunType runType);

    public PlannedWorkout Create(WorkoutDto workout, Athlete athlete, DateOnly date)
    {
        ArgumentNullException.ThrowIfNull(workout);
        ArgumentNullException.ThrowIfNull(athlete);

        var instance = CreateInstance();
        PopulateCommon(instance, workout, athlete, date);
        return instance;
    }

    protected abstract PlannedWorkout CreateInstance();

    protected virtual void PopulateCommon(PlannedWorkout instance, WorkoutDto workout, Athlete athlete, DateOnly date)
    {      
        instance.RunType = workout.RunType;
        instance.TotalDuration = workout.TotalDuration;
        instance.Pace = athlete.EasyPace;
        instance.Description = workout.Description;
        instance.Date = date;
        instance.WeekNumber = workout.WeekNumber;

        if (instance is IStructuredWorkout qualityWorkout && workout.HasDetails)
        {
            var detailsCollection = workout.DetailsCollection!;
            qualityWorkout.DetailsCollection = detailsCollection.ToWorkoutDetails(athlete);
            qualityWorkout.CoolDown = athlete.AthletePreferences.GetPreferencesForRunType(qualityWorkout.RunType).CoolDownDuration ?? TimeSpan.FromSeconds(0);
            var warmUp = athlete.AthletePreferences.GetPreferencesForRunType(qualityWorkout.RunType).WarmUpDuration ?? TimeSpan.FromSeconds(0);
            if (qualityWorkout.RunType is RunType.Steady or RunType.LongRun)
            {
                int warmUpSeconds = workout.TotalDuration - qualityWorkout.GetDetailsDuration() - (int)qualityWorkout.CoolDown.TotalSeconds;
                warmUp = TimeSpan.FromSeconds(Math.Max(warmUp.TotalSeconds, warmUpSeconds));
            }
            qualityWorkout.WarmUp = warmUp;
        } 

        instance.EstimatedDistance = instance.CalculateEstimatedDistance(athlete);
        instance.EstimatedTime = instance.CalculateEstimatedDuration(athlete.AthletePreferences);
    }
}
using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Models.Workouts;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

public sealed class WorkoutMetadataFactory : IWorkoutMetadataFactory
{
    public WorkoutMetadata Create(PlannedWorkout plannedWorkout, Athlete athlete, IWorkoutPreferences prefs)
    {
        ArgumentNullException.ThrowIfNull(plannedWorkout);
        ArgumentNullException.ThrowIfNull(athlete);
        ArgumentNullException.ThrowIfNull(prefs);

        int estimatedDistance = Math.Max(0, plannedWorkout.CalculateEstimatedDistance(athlete));
        int estimatedDuration = Math.Max(0, plannedWorkout.CalculateEstimatedDuration(athlete.AthletePreferences));

        int warmUp = (int)(prefs.WarmUpDuration?.TotalSeconds ?? 0);
        int coolDown = (int)(prefs.CoolDownDuration?.TotalSeconds ?? 0);

        if(plannedWorkout is IStructuredWorkout structuredWorkout && (structuredWorkout.RunType is RunType.Steady or RunType.LongRun))
        {
            warmUp = (int)structuredWorkout.WarmUp.TotalSeconds;
        }

        return new WorkoutMetadata
        {
            Title = plannedWorkout.Name,
            EstimatedDistanceInMeters = estimatedDistance,
            EstimatedDurationInSeconds = estimatedDuration,
            WarmUpInSeconds = Math.Max(0, warmUp),
            CoolDownInSeconds = Math.Max(0, coolDown)
        };
    }
}
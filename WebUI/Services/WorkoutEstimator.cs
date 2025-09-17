using GarminRunerz.Workout.Services.Models;
using WebUI.Models;

namespace WebUI.Services;

/// <summary>
/// Extracted numeric estimation logic — can be unit tested independently.
/// </summary>
internal static class WorkoutEstimator
{
    public static int EstimateDistance(CustomWorkout workout, Athlete athlete)
    {
        var easySpeed = athlete.EasyPace.ToMeterPerSeconds();
        var tempoSpeed = athlete.SemiMarathonPace.ToMeterPerSeconds();
        var fastSpeed = athlete.VmaPace.ToMeterPerSeconds();
        var workoutSpeed = ConvertPaceToMetrePerSecond(workout.Pace);

        double effortDistance = (double)((workout.RunType == RunType.Tempo) ? tempoSpeed
            : (workout.RunType == RunType.Intervals) ? fastSpeed
            : workoutSpeed) * workout.RunDuration * workout.Repetitions;

        double distance;

        if (workout.RunType is RunType.Tempo or RunType.Intervals)
        {
            var restDistance = workout.CoolDownDuration * workout.Repetitions * (double)easySpeed;
            var warmUpDistance = 15 * 60 * (double)easySpeed;
            var coolDownDistance = 10 * 60 * (double)easySpeed;

            distance = effortDistance + restDistance + warmUpDistance + coolDownDistance;
        }
        else if (workout.Repetitions == 0)
        {
            distance = workout.TotalDuration * (double)easySpeed;
        }
        else
        {
            var restDistance = workout.CoolDownDuration * workout.Repetitions * (double)easySpeed;
            var warmUpDuration = workout.TotalDuration - (workout.RunDuration + workout.CoolDownDuration) * workout.Repetitions;
            var warmUpDistance = warmUpDuration * (double)easySpeed;
            distance = effortDistance + restDistance + warmUpDistance;
        }

        return (int)Math.Ceiling(distance / 100) * 100;
    }

    public static int EstimateDuration(CustomWorkout workout)
    {
        var effortDuration = workout.RunDuration * workout.Repetitions;
        var restDuration = workout.CoolDownDuration * workout.Repetitions;
        var warmUpDuration = 15 * 60;
        var coolDownDuration = 10 * 60;
        return (int)(effortDuration + restDuration + warmUpDuration + coolDownDuration);
    }

    private static decimal ConvertPaceToMetrePerSecond(decimal pace) => 1000 / (pace * 60);
}
using GarminRunerz.Workout.Services.Models;
using MudBlazor;
using System.MudPlanner;

namespace WebUI.Models;

public class PlannedWorkout : CustomWorkout
{
    public DateOnly Date { get; set; }
    public int EstimatedDistance { get; set; }
    public int EstimatedTime { get; set; }
    public new Pace Pace { get; set; }
    public string Name => Repetitions > 0 ? GetNameWithIntervals() : GetNameWithoutIntervals();

    // for deserialization
    public PlannedWorkout()
    {

    }
    public PlannedWorkout(CustomWorkout workout, Athlete athlete)
    {
        Id = workout.Id;
        WeekNumber = workout.WeekNumber;
        RunType = workout.RunType;
        TotalDuration = workout.TotalDuration;
        Repetitions = workout.Repetitions;
        RunDuration = workout.RunDuration;
        CoolDownDuration = workout.CoolDownDuration;
        Pace = Pace.FromMinutesDecimal(workout.Pace);
        Speed = workout.Speed;
        Description = workout.Description;
        EstimatedDistance = CalculEstimatedDistance(workout, athlete);
        EstimatedTime = CalculEstimatedDuration(workout);
    }

    private string GetNameWithoutIntervals()
    {
        double distaceInKm = EstimatedDistance / 1000.0;
        if (RunType is RunType.LongRun)
            return $"W{WeekNumber} Long run ({distaceInKm}km)";
        if (RunType is RunType.Recovery or RunType.Easy)
            return $"W{WeekNumber} Easy run ({distaceInKm}km)";
        if (RunType is RunType.Race)
            return $"Race day!";

        return $"W{WeekNumber} Run ({distaceInKm}km)";
    }

    private string GetNameWithIntervals()
    {
        double distaceInKm = EstimatedDistance / 1000.0;

        if (RunType is RunType.Intervals or RunType.Tempo)
            return $"W{WeekNumber} {Repetitions}x{RunDuration}\" @ {Pace:0.##}min/km";
        if (RunType is RunType.LongRun)
            return $"W{WeekNumber} Long run with {Repetitions}x{RunDuration}\" @ {Pace:0.##}min/km ({distaceInKm}m)";

        return $"W{WeekNumber} Easy run with {RunDuration}\" @ {Pace:0.##}min/km";
    }

    private static int CalculEstimatedDistance(CustomWorkout workout, Athlete athlete)
    {
        var EazySpeed = athlete.EasyPace.ToMeterPerSeconds();
        var tempoSpeed = athlete.SemiMarathonPace.ToMeterPerSeconds();
        var fastSpeed = athlete.VmaPace.ToMeterPerSeconds();
        var workoutSpeed = ConvertPaceToMetrePerSecond(workout.Pace);
        double distance;
        var effortDistance = (double)(workout.RunType == RunType.Tempo ? tempoSpeed
            : workout.RunType == RunType.Intervals ? fastSpeed
            : workoutSpeed) * workout.RunDuration * workout.Repetitions;

        if (workout.RunType is RunType.Tempo or RunType.Intervals)
        {
            var restDistance = workout.CoolDownDuration * workout.Repetitions * (double)EazySpeed;
            var warmUpDistance = 15 * 60 * (double)EazySpeed;
            var coolDownDistance = 10 * 60 * (double)EazySpeed;

            distance = effortDistance + restDistance + warmUpDistance + coolDownDistance;
        }
        else if (workout.Repetitions == 0)
        {
            distance = workout.TotalDuration * (double)EazySpeed;
        }
        else // Easy run with determined total duration and intarvals
        {
            var restDistance = workout.CoolDownDuration * workout.Repetitions * (double)EazySpeed;
            var warmUpDuration = workout.TotalDuration - (workout.RunDuration + workout.CoolDownDuration) * workout.Repetitions;
            var warmUpDistance = warmUpDuration * (double)EazySpeed;
            distance = effortDistance + restDistance + warmUpDistance;
        }
        return (int)Math.Ceiling(distance / 100) * 100;
    }
    private static int CalculEstimatedDuration(CustomWorkout workout)
    {
        var effortDuration = workout.RunDuration * workout.Repetitions;
        var restDuration = workout.CoolDownDuration * workout.Repetitions;
        var warmUpDuration = 15 * 60;
        var coolDownDuration = 10 * 60;
        return (int)(effortDuration + restDuration + warmUpDuration + coolDownDuration);
    }

    private static decimal ConvertPaceToMetrePerSecond(decimal pace)
    {
        // Pace is in min/km
        // Convert to m/s
        return 1000 / (pace * 60);
    }
    public CalendarEvent ToCalendarEvent()
    {
        var color = RunType switch
        {
            RunType.Easy => Color.Info,
            RunType.Recovery => Color.Info,
            RunType.Intervals => Color.Warning,
            RunType.Tempo => Color.Warning,
            RunType.LongRun => Color.Success,
            RunType.Race => Color.Error,
            RunType.Steady => Color.Info,
            RunType.Other => Color.Secondary,
            _ => Color.Secondary,
        };
        return new CalendarEvent(Id, CalendarEventName(), Description, color, Date);
    }

    private string CalendarEventName()
    {
        return $"W{WeekNumber} {RunType.ToString()} {Math.Round(EstimatedDistance / 1000.0, 1)}km";
    }
}

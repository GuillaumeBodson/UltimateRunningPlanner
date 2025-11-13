using System.MudPlanner;
using System.Text.Json.Serialization;
using Toolbox.Utilities;
using WebUI.Models.Workouts;
using static WebUI.Models.Planning.RemovalResult;

namespace WebUI.Models;

public class Planning
{
    public required DateOnly StartDate { get; set; }
    public List<PlannedWorkout> Workouts { get; set; } = []; 
    public TrainingTemplateCollection Template { get; set; } = [];

    [JsonIgnore]
    public List<CalendarEvent> CalendarEvents => Workouts.Select(w => w.ToCalendarEvent()).ToList();

    [JsonIgnore]
    public List<WeekRecap> WeeksRecap => Workouts
        .GroupBy(w => w.WeekNumber)
        .Select(g =>
        {
            var monday = g.Min(w => w.Date).GetMonday();
            return new WeekRecap
            {
                WeekStart = monday,
                WeekEnd = monday.AddDays(6),
                Text = [$"Week {g.Key}", $"{g.Count()} workouts", $"{g.Sum(w => w.EstimatedDistance) / 1000.0:0.##} km"],
                CalendarEvents = g.Select(w => w.ToCalendarEvent()).ToList()
            };
        }).ToList();

    public int GetWeekNumber(DateOnly date)
    {
        if (date < StartDate)
            throw new ArgumentOutOfRangeException(nameof(date), "date cannot be before startDate.");

        var lastDay = Workouts.Max(w => w.Date);

        if (date > lastDay)
            throw new ArgumentOutOfRangeException(nameof(date), "date cannot be after the last planned workout.");

        int daysDifference = date.DayNumber - StartDate.DayNumber;
        return (daysDifference / 7) + 1;
    }

    public RemovalResult RemoveWorkoutById(int id)
    {
        try
        {
            var plannedWorkout = Workouts.FirstOrDefault(w => w.Id == id);

            if (plannedWorkout is null)
                return RemovalResult.Failed($"Workout {id} not found", RemovalFailureReason.WorkoutNotFound);

            Workouts.Remove(plannedWorkout);

            return RemovalResult.Succeeded();
        }
        catch (Exception ex)
        {
            return RemovalResult.Failed($"Removal operation failed: {ex.Message}", RemovalFailureReason.UnexpectedError);
        }
    }

    public sealed record RemovalResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public RemovalFailureReason? FailureReason { get; init; }

        public static RemovalResult Succeeded() => new() { Success = true };
        public static RemovalResult Failed(string message, RemovalFailureReason reason) =>
            new() { Success = false, ErrorMessage = message, FailureReason = reason };

        public enum RemovalFailureReason
        {
            PlanningNotLoaded,
            WorkoutNotFound,
            PersistenceError,
            UnexpectedError
        }
    }
}

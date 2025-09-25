using GarminRunerz.Workout.Services.Models;
using System.MudPlanner;
using System.Text.Json.Serialization;
using Toolbox.Utilities;
using WebUI.Models.Workouts;

namespace WebUI.Models;

public class Planning
{
    public DateOnly StartDate { get; set; }
    public List<PlannedWorkout> Workouts { get; set; } = [];
    public List<CustomWorkout> BaseWorkouts { get; set; } = [];

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

    public Athlete Athlete { get; set; }

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
        
}

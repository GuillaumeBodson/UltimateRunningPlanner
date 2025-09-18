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

    public TrainingTemplate TrainingTemplate { get; set; } = new();
    [JsonIgnore]
    public List<CalendarEvent> CalendarEvents  => Workouts.Select(w => w.ToCalendarEvent()).ToList();
    [JsonIgnore]
    public List<WeekRecap> WeeksRecap => Workouts
                .GroupBy(w => w.WeekNumber)
                .Select(g => new WeekRecap()
                {
                    WeekStart = g.Min(w => w.Date).GetMonday(),
                    WeekEnd = g.Min(w => w.Date).GetMonday().AddDays(6),                    
                    Text = [$"Week {g.Key}", $"{g.Count()} workouts", $"{g.Sum(w => w.EstimatedDistance) / 1000.0:0.##} km"],
                    CalendarEvents = g.Select(w => w.ToCalendarEvent()).ToList()
                }).ToList();

    public Athlete Athlete{ get; set; }

    //public static Planning BuildPlanning(DateOnly startDate, List<CustomWorkout> workouts, Athlete athlete)
    //{        
    //    var planning = new Planning();
    //    var mondayOfweek = startDate.GetMonday();
    //    planning.StartDate = mondayOfweek;
    //    planning.BaseWorkouts = workouts;
    //    planning.Athlete = athlete;

    //    var workoutPerWeeks = planning.BaseWorkouts.GroupBy(w => w.WeekNumber);
    //    int id = 1;
    //    foreach (var wkPerWeek in workoutPerWeeks)
    //    {
    //        var template = planning.TrainingTemplate.GetTrainingDaysQueue(wkPerWeek.Count());
    //        id = (wkPerWeek.Key + 9) * 100;

    //        foreach (var workout in wkPerWeek)
    //        {
    //            DayOfWeek trainingDay = template.Dequeue();
    //            workout.Id = id++;
    //            var plannedWorkout = new PlannedWorkout(workout, planning.Athlete);
    //            if (trainingDay == DayOfWeek.Sunday)
    //            {
    //                plannedWorkout.Date = mondayOfweek.AddDays(6);
    //            }
    //            else
    //            {
    //                plannedWorkout.Date = mondayOfweek.AddDays((int)trainingDay - 1);
    //            }
    //            planning.Workouts.Add(plannedWorkout);
    //        }
    //        mondayOfweek = mondayOfweek.AddDays(7);
    //    }
    //    return planning;
    //}
}

using GarminRunerz.Workout.Services.Models;
using Toolbox.Utilities;
using WebUI.Models;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

public class PlanningBuilder : IPlanningBuilder
{
    private readonly IPlannedWorkoutFactory _workoutCreator;

    public PlanningBuilder(IPlannedWorkoutFactory workoutCreator)
    {
        _workoutCreator = workoutCreator;
    }

    public Planning BuildPlanning(DateOnly startDate, List<CustomWorkout> workouts, Athlete athlete)
    {
        var planning = new Planning();
        var mondayOfweek = startDate.GetMonday();
        planning.StartDate = mondayOfweek;
        planning.BaseWorkouts = workouts;
        planning.Athlete = athlete;

        var workoutPerWeeks = planning.BaseWorkouts.GroupBy(w => w.WeekNumber);
        int id = 1;
        foreach (var wkPerWeek in workoutPerWeeks)
        {
            var template = new Queue<DayOfWeek>(planning.TrainingTemplate[wkPerWeek.Count()]);
            id = (wkPerWeek.Key + 9) * 100;

            foreach (var workout in wkPerWeek)
            {
                DayOfWeek trainingDay = template.Dequeue();
                workout.Id = id++;

                var date = trainingDay == DayOfWeek.Sunday ? mondayOfweek.AddDays(6) : mondayOfweek.AddDays((int)trainingDay - 1);

                var plannedWorkout = _workoutCreator.Create(workout, athlete, date);

                planning.Workouts.Add(plannedWorkout);
            }
            mondayOfweek = mondayOfweek.AddDays(7);
        }
        return planning;
    }
}

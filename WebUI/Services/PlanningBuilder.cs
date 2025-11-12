using GarminRunerz.Workout.Services.Models;
using Toolbox.Utilities;
using WebUI.Models;
using WebUI.Services.Dtos;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

public class PlanningBuilder : IPlanningBuilder
{
    private readonly IPlannedWorkoutFactory _workoutCreator;

    public PlanningBuilder(IPlannedWorkoutFactory workoutCreator)
    {
        _workoutCreator = workoutCreator;
    }

    public Planning BuildPlanning(DateOnly startDate, List<WorkoutDto> workouts, Athlete athlete, List<TrainingTemplate>? trainingTemplates = null)
    {
        var planning = new Planning
        {
            StartDate = startDate.GetMonday(),
            Athlete = athlete
        };

        var mondayOfWeek = planning.StartDate;

        foreach (var weekGroup in workouts.GroupBy(w => w.WeekNumber).OrderBy(g => g.Key))
        {
            int numberOfTrainingDays = weekGroup.Count();
            var template = trainingTemplates?.FirstOrDefault(t => t.TrainingDaysCount == numberOfTrainingDays)
                ?? ResolveDefaultTemplate(numberOfTrainingDays);

            planning.Template.TryAdd(template);

            int idSeed = (weekGroup.Key + 9) * 100;
            foreach (var (workout, day) in template.ScheduleWeek(weekGroup))
            {
                var date = day == DayOfWeek.Sunday
                    ? mondayOfWeek.AddDays(6)
                    : mondayOfWeek.AddDays((int)day - 1);

                var planned = _workoutCreator.Create(workout, athlete, date);
                planned.Id = idSeed++;
                planning.Workouts.Add(planned);
            }

            mondayOfWeek = mondayOfWeek.AddDays(7);
        }

        return planning;
    }

    private static TrainingTemplate ResolveDefaultTemplate(int days) =>
        days switch
        {
            <=4 => TrainingTemplate.Default4(),
            >=5 => TrainingTemplate.Default5(),
        };
}

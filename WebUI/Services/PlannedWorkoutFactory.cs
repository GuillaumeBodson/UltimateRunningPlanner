using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

/// <summary>
/// Registry factory (abstract factory pattern): selects the appropriate concrete creator
/// from the registered creators and delegates creation to it.
/// </summary>
public class PlannedWorkoutFactory : IPlannedWorkoutFactory
{
    private readonly IEnumerable<IPlannedWorkoutCreator> _creators;

    public PlannedWorkoutFactory(IEnumerable<IPlannedWorkoutCreator> creators)
    {
        _creators = creators ?? throw new ArgumentNullException(nameof(creators));
    }

    public PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date)
    {
        if (workout is null) throw new ArgumentNullException(nameof(workout));
        if (athlete is null) throw new ArgumentNullException(nameof(athlete));

        var creator = _creators.FirstOrDefault(c => c.CanCreate(workout.RunType))
            ?? _creators.FirstOrDefault(c => c.CanCreate(RunType.Other));

        if (creator is null)
            throw new InvalidOperationException("No PlannedWorkout creator registered for RunType " + workout.RunType);

        return creator.Create(workout, athlete, date);
    }
}
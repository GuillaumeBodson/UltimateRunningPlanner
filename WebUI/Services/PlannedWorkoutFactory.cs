using GarminRunerz.Workout.Services.Models;
using WebUI.Creators;
using WebUI.Models;
using WebUI.Models.Workouts;
using WebUI.Services.Dtos;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

/// <summary>
/// Registry factory (abstract factory pattern): selects the appropriate concrete creator
/// from the registered creators and delegates creation to it.
/// </summary>
public class PlannedWorkoutFactory : IPlannedWorkoutFactory
{
    private readonly IDictionary<RunType, IPlannedWorkoutCreator> _creators;

    public PlannedWorkoutFactory(IEnumerable<IPlannedWorkoutCreator> creators)
    {
        var runtypes = Enum.GetValues<RunType>();
        _creators = creators.ToDictionary(rt => runtypes.First(x => rt.CanCreate(x)));
        _creators[RunType.Recovery] = _creators[RunType.Easy]; // Recovery is a subtype of Easy
    }

    public PlannedWorkout Create(WorkoutDto workout, Athlete athlete, DateOnly date)
    {
        ArgumentNullException.ThrowIfNull(workout);
        ArgumentNullException.ThrowIfNull(athlete);

        if (!_creators.TryGetValue(workout.RunType, out var creator) || creator is null)
        {
            throw new InvalidOperationException("No PlannedWorkout creator registered for RunType " + workout.RunType);
        }

        return creator.Create(workout, athlete, date);
    }
}
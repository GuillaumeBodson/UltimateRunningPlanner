namespace User.API.Mappers;

using GarminRunerz.Workout.Services.Models;
using User.API.Data.Models;
using User.API.Dtos;

public sealed class Mapper : IModelDtoMapper
{
    // AthletePreferences
    public AthletePreferencesDto ToDto(AthletePreferences model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new AthletePreferencesDto
        {
            Id = model.Id,
            WarmUpDuration = model.WarmUpDuration,
            CoolDownDuration = model.CoolDownDuration
        };
    }

    // Athlete
    public AthleteDto ToDto(Athlete model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new AthleteDto
        {
            Id = model.Id,
            EasyPace = model.EasyPace,
            MarathonPace = model.MarathonPace,
            SemiMarathonPace = model.SemiMarathonPace,
            VmaPace = model.VmaPace,
            TrainingTemplates = CloneTemplates(model.TrainingTemplates)
        };
    }

    // User
    public UserDto ToDto(User model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new UserDto
        {
            Id = model.Id,
            Athlete = ToDto(model.Athlete),
            AthletePreferences = ToDto(model.AthletePreferences)
        };
    }

    private static HashSet<Dictionary<DayOfWeek, RunType?>> CloneTemplates(HashSet<Dictionary<DayOfWeek, RunType?>> templates)
    {
        ArgumentNullException.ThrowIfNull(templates);
        return templates.Count == 0
            ? []
            : templates.Select(t => new Dictionary<DayOfWeek, RunType?>(t)).ToHashSet();
    }
}

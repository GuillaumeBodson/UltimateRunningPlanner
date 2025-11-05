using User.API.Data.Models;
using User.API.Dtos;

namespace User.API.Mappers;

public interface IModelDtoMapper
{
    // AthletePreferences
    AthletePreferencesDto ToDto(AthletePreferences model);

    // Athlete
    AthleteDto ToDto(Athlete model);

    // User
    UserDto ToDto(Data.Models.User model);
}
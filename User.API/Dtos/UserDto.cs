namespace User.API.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public AthleteDto Athlete { get; set; } = null!;
    public AthletePreferencesDto AthletePreferences { get; set; } = null!;
}

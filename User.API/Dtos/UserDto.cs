namespace User.API.Dtos;

public record UserDto
{
    public int Id { get; set; }
    public AthleteDto Athlete { get; set; } = null!;
    public AthletePreferencesDto AthletePreferences { get; set; } = null!;
}

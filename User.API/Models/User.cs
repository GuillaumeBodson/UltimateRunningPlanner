namespace User.API.Models;

public class User
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public Athlete Athlete { get; set; } = null!;
    public int AthletePreferencesId { get; set; }
    public AthletePreferences AthletePreferences { get; set; } = null!;
}

namespace User.API.Models;

public class User
{
    public int Id { get; set; }
    public Athlete Athlete { get; set; } = null!;
    public AthletePreferences AthletePreferences { get; set; } = null!;
}

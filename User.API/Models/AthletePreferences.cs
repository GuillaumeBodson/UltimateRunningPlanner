namespace User.API.Models;

public class AthletePreferences
{
    public int Id { get; set; }
    public int WarmUpDuration { get; set; }
    public int CoolDownDuration { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}

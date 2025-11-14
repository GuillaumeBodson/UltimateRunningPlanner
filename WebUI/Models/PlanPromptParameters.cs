namespace WebUI.Models;

public enum RaceDistance
{
    Marathon = 42195,
    HalfMarathon = 21097,
    TenK = 10000,
    FiveK = 5000
}

public sealed class PlanPhaseConfig
{
    public string Name { get; init; } = string.Empty;
    public int StartWeek { get; init; }
    public int EndWeek { get; init; }
    public int RunsPerWeek { get; init; }
    public string FocusDescription { get; init; } = string.Empty;
}

public sealed class PlanPromptParameters
{
    public RaceDistance Distance { get; set; }
    public DateTime? RaceDate { get; set; }
    public DateTime? StartDate { get; set; }
    public string GoalTimeOrPaceDescription { get; set; } = string.Empty;
    public int MaxWeeklyKm { get; set; }
    public bool WorkoutsInTimeNotDistance { get; set; } = true;

    // Default 21-week marathon structure (matches example)
    public List<PlanPhaseConfig> Phases { get; } =
    [
        new() { Name = "Base", StartWeek = 1, EndWeek = 6, RunsPerWeek = 4, FocusDescription = "faster speeds are prioritize (MAS and FiveK)" },
        new() { Name = "Transition", StartWeek = 7, EndWeek = 13, RunsPerWeek = 4, FocusDescription = "alternate 4 & 5 runs/week, transition to slower speed (TenK and Semi-marathon)" },
        new() { Name = "Marathon-specific", StartWeek = 14, EndWeek = 19, RunsPerWeek = 5, FocusDescription = "marathon-specific work" },
        new() { Name = "Taper", StartWeek = 20, EndWeek = 22, RunsPerWeek = 4, FocusDescription = "reduce volume & sharpen" }
    ];

    public bool IsValid(out string? error)
    {
        if (StartDate >= RaceDate)
        {
            error = "Start date must be before race date.";
            return false;
        }
        if (MaxWeeklyKm <= 0 || MaxWeeklyKm > 300)
        {
            error = "Max weekly km is out of reasonable bounds.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(GoalTimeOrPaceDescription))
        {
            error = "Goal description is required.";
            return false;
        }
        error = null;
        return true;
    }
}

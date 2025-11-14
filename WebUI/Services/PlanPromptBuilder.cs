using System.Globalization;
using System.Text;
using System.Text.Json;
using WebUI.Models;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

public sealed class PlanPromptBuilder : IPlanPromptBuilder
{
    public string BuildPrompt(Athlete athlete, PlanPromptParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(athlete);
        ArgumentNullException.ThrowIfNull(parameters);

        var sb = new StringBuilder();

        sb.AppendLine($"I want you to create a full {TotalWeeks(parameters)}-week {(parameters.Distance == RaceDistance.Marathon ? "marathon" : parameters.Distance.ToString().ToLower(CultureInfo.InvariantCulture))} training plan to run a {(parameters.Distance == RaceDistance.Marathon ? "marathon" : parameters.Distance.ToString().ToLower())} at {parameters.GoalTimeOrPaceDescription}.");
        sb.AppendLine();
        sb.AppendLine("My background:");

        // Simple background derivation (could be enriched from Performances later)
        var halfPerf = athlete.Performances?.FirstOrDefault(p => p.DistanceMeters is 21097);
        if (halfPerf is not null)
        {
            sb.AppendLine($"– I recently ran a half marathon in {FormatTime(halfPerf.TimeSeconds)}.");
        }
        else
        {
            sb.AppendLine("– I have recent race performances stored.");
        }

        // Estimate current weekly frequency from templates or performances
        int currentWeeklyRuns = athlete.TrainingTemplates.OrderByDescending(t => t.TrainingDaysCount).FirstOrDefault()?.TrainingDaysCount ?? 4;
        sb.AppendLine($"– I trained {currentWeeklyRuns} times per week recently.");
        sb.AppendLine($"– For this build-up, start with {parameters.Phases[0].RunsPerWeek} runs/week and follow the phase structure below.");
        if (parameters.WorkoutsInTimeNotDistance)
            sb.AppendLine("– I prefer workouts described in time (minutes or seconds), not in distance.");
        sb.AppendLine("– To build the workouts use the paces in the provided .json file \"paces.json\"");
        sb.AppendLine();

        sb.AppendLine("Requirements:");
        sb.AppendLine("1. Structure the plan into phases:");
        foreach (var phase in parameters.Phases)
        {
            sb.AppendLine($"   - Weeks {phase.StartWeek}–{phase.EndWeek}: {phase.Name} phase ({phase.RunsPerWeek} runs/week){(string.IsNullOrWhiteSpace(phase.FocusDescription) ? "" : $" {phase.FocusDescription}")}");
        }

        sb.AppendLine("2. Include the following run types: Easy, Steady, Tempo, Intervals, LongRun.");
        sb.AppendLine("3. Include one LongRun every week, progressively increasing duration and including MP blocks.");
        sb.AppendLine("4. Each workout must have: total duration, number of repetitions, duration per rep, recovery duration, pace, and a short description.");
        sb.AppendLine("5. Output the plan as a CSV file with the columns:");
        sb.AppendLine("   Week, RunType, TotalDuration(s), Details, Description");
        sb.AppendLine("8. Details column is used to specify repetitions (leave blank if no repetition).");
        sb.AppendLine("9. Easy runs don't have repetitions (details column is blank).");
        sb.AppendLine("10. The possible workout's format of details:");
        sb.AppendLine("\ta) n * (t, p, r)");
        sb.AppendLine("\tb) n1 * (t1, p1, r1),..., nx * (tx, px, rx)");
        sb.AppendLine("\tc) n1 * (n2 * (t, p, r2), r1)");
        sb.AppendLine("    pace should be specified by keyword from \"paces.json\"");
        sb.AppendLine("11. For Interval and Tempo workout, don't put warm up and cool down in the details.");
        sb.AppendLine($"12. Max km per week is {parameters.MaxWeeklyKm}.");
        sb.AppendLine("13. For Interval workouts, vary pace (e.g. early weeks use MAS or FiveK pace).");
        sb.AppendLine("14. Do not use more than 1 Intervals or Tempo workout per week.");
        sb.AppendLine("15. Use quoted fields for the .csv file.");
        sb.AppendLine();
        sb.AppendLine("If a short workout includes only 1 block of marathon pace, count it as a Steady workout.");
        sb.AppendLine();
        sb.AppendLine($"Start date: {parameters.StartDate:yyyy-MM-dd}. Race date: {parameters.RaceDate:yyyy-MM-dd}.");
        sb.AppendLine();
        sb.AppendLine("Finally, generate a downloadable CSV file named plan.csv.");

        return sb.ToString();
    }

    public string BuildPacesJson(Athlete athlete)
    {
        ArgumentNullException.ThrowIfNull(athlete);
        var paces = new Dictionary<string, string>
        {
            ["EasyPace"] = athlete.EasyPace.ToString(),
            ["MarathonPace"] = athlete.MarathonPace.ToString(),
            ["SemiMarathonPace"] = athlete.SemiMarathonPace.ToString(),
            ["TenKPace"] = athlete.TenKPace.ToString(),
            ["FiveKPace"] = athlete.FiveKPace.ToString(),
            ["MASPace"] = athlete.MasPace.ToString()
        };
        return JsonSerializer.Serialize(paces, new JsonSerializerOptions { WriteIndented = true });
    }

    private static int TotalWeeks(PlanPromptParameters parameters)
        => parameters.Phases.Max(p => p.EndWeek);

    private static string FormatTime(int totalSeconds)
    {
        var ts = TimeSpan.FromSeconds(totalSeconds);
        return ts.Hours > 0
            ? $"{ts.Hours}h{ts.Minutes:00}"
            : $"{ts.Minutes}m{ts.Seconds:00}";
    }
}

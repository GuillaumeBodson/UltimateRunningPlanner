using WebUI.Models;

namespace WebUI.Services.Interfaces;

public interface IPlanPromptBuilder
{
    string BuildPrompt(Athlete athlete, PlanPromptParameters parameters);
    string BuildPacesJson(Athlete athlete);
}

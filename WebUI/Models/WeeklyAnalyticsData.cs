namespace WebUI.Models;

public class WeeklyAnalyticsData
{
    public string WeekLabel { get; set; } = "";
    public DateOnly WeekStart { get; set; }
    public double TotalDistance { get; set; }
    public double IntervalDistance { get; set; }
}
namespace LMS.Web.ViewModels;

public class ScheduleItemViewModel
{
    public string Day { get; set; } = null!;
    public string StartTime { get; set; } = null!;
    public string EndTime { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Teacher { get; set; } = null!;
}

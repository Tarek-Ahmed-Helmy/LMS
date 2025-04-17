namespace LMS.Web.ViewModels;

public class StudentScheduleViewModel
{
    public string FullName { get; set; } = null!;
    public string StudentId { get; set; } = null!;
    public Dictionary<string, List<ScheduleItemViewModel>> GroupedSchedule { get; set; } = new();
}

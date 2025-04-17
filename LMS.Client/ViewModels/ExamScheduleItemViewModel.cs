namespace LMS.Web.ViewModels;

public class ExamScheduleItemViewModel
{
    public string Subject { get; set; } = null!;
    public string Teacher { get; set; } = null!;
    public string Date { get; set; } = null!;
    public int TotalMarks { get; set; }
    public int DurationMinutes { get; set; }
}

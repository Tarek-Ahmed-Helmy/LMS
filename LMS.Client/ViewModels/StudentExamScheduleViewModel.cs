namespace LMS.Web.ViewModels;

public class StudentExamScheduleViewModel
{
    public string StudentId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public Dictionary<string, List<ExamScheduleItemViewModel>> GroupedExams { get; set; } = new();
}

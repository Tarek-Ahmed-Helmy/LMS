namespace LMS.Web.ViewModels;
public class ExamResultVM
{
    public int ExamResultId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int Score { get; set; }
    public string? Remarks { get; set; }
}
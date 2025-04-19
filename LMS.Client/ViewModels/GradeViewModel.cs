namespace LMS.Web.ViewModels;

public class GradeViewModel
{
    public string Subject { get; set; } = string.Empty;
    public string Student { get; set; } = string.Empty;
    public string Score { get; set; }
    public int TotalMarks { get; set; }
    public string Feedback { get; set; } = string.Empty;
}

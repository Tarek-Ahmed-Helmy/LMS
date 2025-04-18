namespace LMS.Web.ViewModels;

public class AssignedQuizViewModel
{
    public int ExamId { get; set; }
    public string SubjectName { get; set; }
    public DateTime ExamDate { get; set; }
    public int TotalMarks { get; set; }
    public bool IsSubmitted { get; set; }
}


namespace LMS.Web.ViewModels;

public class QuizDetailsViewModel
{
    public int ExamId { get; set; }
    public string SubjectName { get; set; }
    public DateTime ExamDate { get; set; }
    public int TotalMarks { get; set; }
    public int DurationInMinutes { get; set; }
    public string TeacherName { get; set; }
    public bool IsSubmitted { get; set; }
}


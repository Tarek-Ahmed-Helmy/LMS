using LMS.Entities.Models;

namespace LMS.Web.ViewModels;

public class QuizListViewModel
{
    public int ExamID { get; set; }
    public string SubjectName { get; set; }
    public string ClassNumber { get; set; }
    public DateTime ExamDate { get; set; }
    public int TotalMarks { get; set; }
    public int Duration { get; set; }
}

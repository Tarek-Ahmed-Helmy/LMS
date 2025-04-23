namespace LMS.Web.ViewModels;

public class StudentExamResultsViewModel
{
    public string StudentId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public List<ExamResultViewModel> Results { get; set; } = new();
}

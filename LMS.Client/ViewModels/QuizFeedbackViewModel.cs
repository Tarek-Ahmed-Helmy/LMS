namespace LMS.Web.ViewModels;

public class QuizFeedbackViewModel
{
    public int QuizId { get; set; }
    public string SubjectName { get; set; }
    public int TotalMarks { get; set; }
    public int StudentScore { get; set; }

    public List<QuizFeedbackItem> FeedbackItems { get; set; } = new();
}

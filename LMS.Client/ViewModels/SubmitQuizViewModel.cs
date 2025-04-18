namespace LMS.Web.ViewModels;

public class SubmitQuizViewModel
{
    public int QuizId { get; set; }

    public List<QuizQuestionAnswer> Questions { get; set; } = new();
}

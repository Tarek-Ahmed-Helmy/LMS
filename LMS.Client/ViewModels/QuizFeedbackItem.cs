namespace LMS.Web.ViewModels;

public class QuizFeedbackItem
{
    public string QuestionText { get; set; }
    public string CorrectAnswer { get; set; }
    public string SelectedAnswer { get; set; }
    public bool IsCorrect => SelectedAnswer == CorrectAnswer;
}

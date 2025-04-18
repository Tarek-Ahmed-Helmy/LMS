using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class QuizQuestionAnswer
{
    public int QuestionId { get; set; }
    public string? QuestionText { get; set; }
    public string? OptionA { get; set; }
    public string? OptionB { get; set; }
    public string? OptionC { get; set; }
    public string? OptionD { get; set; }

    [Required]
    public string SelectedAnswer { get; set; }
}

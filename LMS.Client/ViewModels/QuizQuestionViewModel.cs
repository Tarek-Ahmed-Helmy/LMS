using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class QuizQuestionViewModel
{
    public int Index { get; set; } // for display purposes
    public int? QuizQuestionId { get; set; } // optional for new question

    [Required]
    public string QuestionText { get; set; }

    [Required]
    public string OptionA { get; set; }

    [Required]
    public string OptionB { get; set; }

    [Required]
    public string OptionC { get; set; }

    [Required]
    public string OptionD { get; set; }

    [Required]
    [Display(Name = "Correct Answer")]
    [RegularExpression("A|B|C|D")]
    public string CorrectAnswer { get; set; }
}

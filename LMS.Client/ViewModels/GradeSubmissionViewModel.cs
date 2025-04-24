namespace LMS.Web.ViewModels;

using System.ComponentModel.DataAnnotations;
public class GradeSubmissionViewModel
{
    [Required, Range(0, 100)]
    public int Score { get; set; }

    [DataType(DataType.MultilineText)]
    public string? Feedback { get; set; }
}
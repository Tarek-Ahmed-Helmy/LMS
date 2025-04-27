namespace LMS.Web.ViewModels;

using System.ComponentModel.DataAnnotations;
public class GradeSubmissionViewModel
{
    public string? AssignmentTitle { get; set; }
    public string AssignmentDescription { get; set; } = string.Empty;
    [Required, Range(0, 100)]
    public int Score { get; set; }

    [DataType(DataType.MultilineText)]
    public string? Feedback { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels.StudentViewModels;

    public class SubmissionFeedbackViewModel
    {
    [Required(ErrorMessage = "Student full name is required.")]
    public string StudentFullName { get; set; }

    [Required(ErrorMessage = "Assignment title is required.")]
    public string AssignmentTitle { get; set; }

    [Required(ErrorMessage = "Submission date is required.")]
    public DateTime SubmissionDate { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Score must be a valid number.")]
    public int Score { get; set; }

    [Url(ErrorMessage = "Please enter a valid file URL.")]
    public string? FilePath { get; set; }

    [StringLength(500, ErrorMessage = "Feedback cannot exceed 500 characters.")]
    public string? Feedback { get; set; }
}


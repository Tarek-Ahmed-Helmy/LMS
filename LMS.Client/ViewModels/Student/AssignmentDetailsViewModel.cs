using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels.StudentViewModels;

public class AssignmentDetailsViewModel
{
    public int AssignmentId { get; set; }

    [Display(Name = "Title")]
    public required string Title { get; set; }

    [Display(Name = "Instructions")]
    public string? Description { get; set; }
    [Display(Name = "Subject")]
    public string Subject { get; set; } = "N/A";

    [Display(Name = "Teacher")]
    public string Teacher { get; set; } = "N/A";

    [Display(Name = "Mode")]
    public string Mode { get; set; } = "Online";

    [Display(Name = "Progress")]
    public string Progress { get; set; } = "Not Submitted";

    [Display(Name = "Grade")]
    public string Grade { get; set; } = "--";

    public string StudentId { get; set; } = string.Empty;

    public DateTime? SubmissionDate { get; set; }
    public DateTime ExpiresAt { get; set; }

    public string? FilePath { get; set; }
}


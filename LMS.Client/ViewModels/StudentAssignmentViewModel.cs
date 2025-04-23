using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class StudentAssignmentViewModel
{
    [Display(Name = "Assignment")]
    public required string Title { get; set; }

    [Display(Name = "Progress")]
    public required string Progress { get; set; } // Not Submitted | Submitted - Pending Grading | Graded

    [Display(Name = "Grade")]
    public required string Grade { get; set; } //  10 / 10 or --

    [Display(Name = "Subject")]
    public required string Subject { get; set; }

    [Display(Name = "Teacher")]
    public required string Teacher { get; set; }

    [Display(Name = "Expires At")]
    public DateTime ExpiresAt { get; set; }

    public int AssignmentId { get; set; } 
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Entities.Models;

public class Assignment
{
    public int AssignmentId { get; set; }
    public string Title { get; set; }

    [DisplayName("Description")]
    public string AssignmentDescription { get; set; }
    public DateTime Deadline { get; set; }

    [DisplayName("Total Marks")]
    public int TotalMarks { get; set; }

    [DisplayName("Submission Type")]
    public string SubmissionType { get; set; }

    [DisplayName("Status")]
    public string AssignmentStatus { get; set; }

    [ForeignKey("Teacher")]
    public int TeacherId { get; set; }

    [ForeignKey("Subject")]
    public int SubjectId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ValidateNever]
    public Teacher? Teacher { get; set; }

    [ValidateNever]
    public Subject? Subject { get; set; }
}

// SubmissionType CHECK ('Online', 'Offline')
// AssignmentStatus CHECK ('Pending', 'Completed', 'Overdue')
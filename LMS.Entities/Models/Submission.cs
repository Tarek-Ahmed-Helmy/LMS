using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LMS.Entities.Models;

public class Submission
{
    public int SubmissionId { get; set; }

    [DisplayName("Submission Date")]
    public DateTime SubmissionDate { get; set; }

    [Required]
    [DisplayName("Attached File")]
    public required string FilePath { get; set; }
    public int Score { get; set; }
    public string? Feedback { get; set; }

    [ForeignKey("Assignment")]
    public int AssignmentId { get; set; }

    [ForeignKey("Student")]
    public required string StudentId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public Assignment? Assignment { get; set; }

    [ValidateNever]
    public Student? Student { get; set; }
}
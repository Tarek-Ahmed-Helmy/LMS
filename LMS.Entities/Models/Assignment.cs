using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Entities.Models;

public class Assignment
{
    public int AssignmentId { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    [DisplayName("Description")]
    public required string AssignmentDescription { get; set; }
    public DateTime Deadline { get; set; }

    [Required]
    [DisplayName("Total Marks")]
    public int TotalMarks { get; set; }

    [DisplayName("Submission Type")]
    [EnumDataType(typeof(SubmissionType))]
    public SubmissionType SubmissionType { get; set; }

    [ForeignKey("Teacher")]
    public required string TeacherId { get; set; }

    [ForeignKey("Subject")]
    public int SubjectId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public Teacher? Teacher { get; set; }

    [ValidateNever]
    public Subject? Subject { get; set; }

    [ValidateNever]
    public ICollection<Submission>? Submissions { get; set; }
}

public enum SubmissionType
{
    Online,
    Offline
}
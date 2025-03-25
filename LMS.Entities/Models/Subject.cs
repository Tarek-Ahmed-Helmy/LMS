using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LMS.Entities.Models;

public class Subject
{
    public int SubjectId { get; set; }

    [Required]
    [DisplayName("Subject Name")]
    public required string SubjectName { get; set; }

    [Required]
    [DisplayName("Code")]
    public required string SubjectCode { get; set; } // Unique (custom validation)

    [Required]
    [DisplayName("Description")]
    public required string SubjectDescription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public ICollection<Resource>? Resources { get; set; }

    [ValidateNever]
    public ICollection<Assignment>? Assignments { get; set; }

    [ValidateNever]
    public ICollection<Exam>? Exams { get; set; }
}
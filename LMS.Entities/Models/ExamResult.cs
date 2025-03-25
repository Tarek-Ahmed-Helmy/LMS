using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Entities.Models;

public class ExamResult
{
    public int ExamResultID { get; set; }
    public string? Remarks { get; set; }
    public int Score { get; set; }

    [ForeignKey("Exam")]
    public int ExamId { get; set; }

    [ForeignKey("Student")]    
    public required string StudentID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public Exam? Exam { get; set; }

    [ValidateNever]
    public Student? Student { get; set; }
}
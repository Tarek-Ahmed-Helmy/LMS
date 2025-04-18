using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Entities.Models;

public class Exam
{
    public int ExamID { get; set; }

    [Required]
    [EnumDataType(typeof(ExamType))]
    public ExamType ExamType { get; set; }

    [Required]
    public DateTime ExamDate { get; set; }

    [Required]
    [Display(Name = "Total Marks")]
    public int TotalMarks { get; set; }

    [Required]
    [Display(Name = "Exam Duration (in minutes)")]
    public int ExamDuration { get; set; }

    [ForeignKey("Subject")]
    public int SubjectID { get; set; }

    [ForeignKey("Class")]
    public int ClassID { get; set; }

    [ForeignKey("Teacher")]
    public required string TeacherID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public Subject? Subject { get; set; }

    [ValidateNever]
    public Class? Class { get; set; }

    [ValidateNever]
    public Teacher? Teacher { get; set; }

    [ValidateNever]
    public ICollection<ExamResult>? ExamResults { get; set; }

    [ValidateNever]
    public ICollection<QuizQuestion>? quizQuestions { get; set; }
}

public enum ExamType
{
    Midterm, Final, Quiz
}

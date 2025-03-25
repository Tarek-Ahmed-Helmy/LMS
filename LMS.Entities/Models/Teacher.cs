using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Entities.Models;

public class Teacher
{
    [ForeignKey("ApplicationUser")]
    public required string TeacherId { get; set; }
    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    [Required]
    public required string Qualification { get; set; }

    [Display(Name = "Experience (in years)")]
    public int Experience { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; }

    [ValidateNever]
    public ICollection<Schedule>? Schedules { get; set; }

    [ValidateNever]
    public ICollection<Resource>? Resources { get; set; }

    [ValidateNever]
    public ICollection<Assignment>? Assignments { get; set; }

    [ValidateNever]
    public ICollection<Exam>? Exams { get; set; }
}
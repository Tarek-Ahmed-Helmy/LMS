using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Entities.Models;

public class Student
{
    [ForeignKey("ApplicationUser")]
    public required string StudentId { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }

    [Required]
    [StringLength(20)]
    public required string EmergencyContact { get; set; }

    [Required]
    public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(255)]
    public required string StudentNumber { get; set; }

    [ForeignKey("Class")]
    public int? ClassId { get; set; }

    [ForeignKey("Parent")]
    public string? ParentId { get; set; }

    [ForeignKey("Bus")]
    public int? BusId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; }

    [ValidateNever]
    public Class? Class { get; set; }

    [ValidateNever]
    public Parent? Parent { get; set; }

    [ValidateNever]
    public Bus? Bus { get; set; }

    [ValidateNever]
    public ICollection<Attendance>? Attendances { get; set; }

    [ValidateNever]
    public ICollection<Payment>? Payments { get; set; }

    [ValidateNever]
    public ICollection<ExamResult>? ExamResults { get; set; }

    [ValidateNever]
    public ICollection<Submission>? Submissions { get; set; }
}

public enum Gender
{
    male,
    female
}
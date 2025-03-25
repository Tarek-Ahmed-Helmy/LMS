using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Entities.Models;

public class Attendance
{
    public int AttendanceID { get; set; }

    [Required]
    [ForeignKey("ApplicationUser")]
    public required string StudentId { get; set; }
    public DateTime Date { get; set; }

    [Required]
    [EnumDataType(typeof(AttendanceStatus))]
    public AttendanceStatus Status { get; set; }

    [Required]
    [StringLength(255)]
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; }
}

public enum AttendanceStatus
{
    Present,
    Absent,
    Late
}
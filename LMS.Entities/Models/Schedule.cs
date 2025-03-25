using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Entities.Models;

public class Schedule
{
    public int ScheduleId { get; set; }

    [Required, MaxLength(20)]
    [EnumDataType(typeof(DayOfWeek))]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

    [Required]
    public int ClassId { get; set; }

    [Required]
    public required string TeacherId { get; set; }

    [Required]
    public int SubjectId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ClassId")]
    public virtual Class? Class { get; set; }

    [ForeignKey("TeacherId")]
    public virtual Teacher? Teacher { get; set; }

    [ForeignKey("SubjectId")]
    public virtual Subject? Subject { get; set; }
}
public enum DayOfWeek
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday
}
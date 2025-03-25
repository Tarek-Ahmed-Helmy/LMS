using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace LMS.Entities.Models;

public class Class
{
    public int ClassId { get; set; }

    [Required]
    [EnumDataType(typeof(GradeLevel))]
    public GradeLevel GradeLevel { get; set; }

    [Required]
    public int Capacity { get; set; }

    [Required]
    [StringLength(255)]
    public required string ClassNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ValidateNever]
    public ICollection<Student>? Students { get; set; }

    [ValidateNever]
    public ICollection<Schedule>? Schedules { get; set; }

    [ValidateNever]
    public ICollection<Exam>? Exams { get; set; }
}

public enum GradeLevel
{
    Kindergarten = 0,
    Grade1 = 1,
    Grade2 = 2,
    Grade3 = 3,
    Grade4 = 4,
    Grade5 = 5,
    Grade6 = 6,
    Grade7 = 7,
    Grade8 = 8,
    Grade9 = 9,
    Grade10 = 10,
    Grade11 = 11,
    Grade12 = 12
}

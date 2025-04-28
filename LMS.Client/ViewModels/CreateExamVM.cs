using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;
namespace LMS.Web.ViewModels;
public class CreateExamVM
{
    [Required]
    public ExamType ExamType { get; set; }

    [Required, DataType(DataType.DateTime)]
    public DateTime ExamDate { get; set; } = DateTime.UtcNow.AddDays(7);

    [Required, Range(1, 500)]
    public int TotalMarks { get; set; }

    [Required, Range(10, 240)]
    public int ExamDuration { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }

    [Required]
    public int ClassId { get; set; }

    /* Drop-downs */
    public IEnumerable<Subject>? Subjects { get; set; }
    public IEnumerable<Class>? Classes { get; set; }
}
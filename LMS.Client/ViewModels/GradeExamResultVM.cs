using System.ComponentModel.DataAnnotations;
namespace LMS.Web.ViewModels;

public class GradeExamResultVM
{
    public int ExamResultId { get; set; }
    public int ExamId { get; set; }
    public string? StudentName { get; set; }
    [Required, Range(0, 500)]
    public int Score { get; set; }

    public string? Remarks { get; set; }
}
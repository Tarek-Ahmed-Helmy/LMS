using LMS.Web.ViewModels;
using System.ComponentModel.DataAnnotations;
namespace LMS.Web.ViewModels;

public class GradeExamResultVM
{
    [Required, Range(0, 500)]
    public int Score { get; set; }

    public string? Remarks { get; set; }
}
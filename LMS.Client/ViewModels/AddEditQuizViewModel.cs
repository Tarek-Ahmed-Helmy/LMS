using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class AddEditQuizViewModel
{
    public int? ExamID { get; set; }

    [Required]
    [Display(Name = "Exam Date")]
    public DateTime ExamDate { get; set; }

    [Required]
    [Display(Name = "Total Marks")]
    public int TotalMarks { get; set; }

    [Required]
    [Display(Name = "Exam Duration (minutes)")]
    public int ExamDuration { get; set; }

    [Required]
    public int SubjectID { get; set; }

    [Required]
    public int ClassID { get; set; }

    public List<QuizQuestionViewModel> Questions { get; set; } = new();

    public List<Subject?>? Subjects { get; set; }
    public List<Class?>? Classes { get; set; }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Entities.Models;

public class StudentAnswer
{
    public int StudentAnswerId { get; set; }

    public string StudentId { get; set; }

    public int QuizQuestionId { get; set; }

    [Required]
    public string SelectedAnswer { get; set; }

    [ForeignKey("StudentId")]
    public Student Student { get; set; }

    [ForeignKey("QuizQuestionId")]
    public QuizQuestion Question { get; set; }
}


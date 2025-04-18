using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LMS.Entities.Models;

public class QuizQuestion
{
    public int QuizQuestionId { get; set; }

    [Required]
    public string QuestionText { get; set; }

    [Required]
    public string OptionA { get; set; }

    [Required]
    public string OptionB { get; set; }

    [Required]
    public string OptionC { get; set; }

    [Required]
    public string OptionD { get; set; }

    [Required]
    public string CorrectAnswer { get; set; } // A, B, C, or D

    public int ExamID { get; set; }

    [ForeignKey("ExamID")]
    public Exam Exam { get; set; }

    [ValidateNever]
    public ICollection<StudentAnswer>? Answers { get; set; }
}


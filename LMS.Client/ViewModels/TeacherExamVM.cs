using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;
public class TeacherExamVM
{
    public int ExamId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public int TotalMarks { get; set; }
    public string Status { get; set; } = string.Empty;
}
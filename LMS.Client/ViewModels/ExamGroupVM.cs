using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class ExamGroupVM
{
    public string SubjectName { get; set; } = string.Empty;
    public IList<TeacherExamVM> Exams { get; set; } = new List<TeacherExamVM>();
}
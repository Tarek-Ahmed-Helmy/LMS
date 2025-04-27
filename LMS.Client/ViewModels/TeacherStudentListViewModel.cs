using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class TeacherStudentListViewModel
{
    [Display(Name = "Student ID")]
    public string StudentId { get; set; }

    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Display(Name = "Student Number")]
    public string StudentNumber { get; set; }

    [Display(Name = "Class")]
    public string ClassNumber { get; set; }

    public GradeLevel GradeLevel { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels.Teacher;

public class TeacherStudentListViewModel
{
    [Display(Name = "Student ID")]
    public string StudentId { get; set; }

    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Display(Name = "Student Number")]
    public string StudentNumber { get; set; }

    [Display(Name = "Class")]
    public string ClassName { get; set; }
}

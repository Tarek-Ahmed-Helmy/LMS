using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;
namespace LMS.Web.ViewModels;

public class AssignmentGroupViewModel
{
    public string SubjectName { get; set; } = string.Empty;
    public IList<TeacherAssignmentViewModel> Assignments { get; set; } = new List<TeacherAssignmentViewModel>();
}
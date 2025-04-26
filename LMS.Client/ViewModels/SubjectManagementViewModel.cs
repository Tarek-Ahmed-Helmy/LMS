using LMS.Entities.Models;

namespace LMS.Web.ViewModels;

public class SubjectManagementViewModel
{
    public IEnumerable<Subject> Subjects { get; set; }
    public SubjectViewModel NewSubject { get; set; }
}

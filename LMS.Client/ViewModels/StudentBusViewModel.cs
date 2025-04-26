using LMS.Entities.Models;

namespace LMS.Web.ViewModels;

public class StudentBusViewModel
{
    public string Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string? ClassNumber { get; set; }
    public GradeLevel GradeLevel { get; set; }
    public string? EmergencyContact { get; set; }
}

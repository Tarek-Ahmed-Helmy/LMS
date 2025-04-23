using LMS.Entities.Models;

namespace LMS.Web.ViewModels;

public class TeacherResourceItemViewModel
{
    public int ResourceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ResourceType ResourceType { get; set; }
}
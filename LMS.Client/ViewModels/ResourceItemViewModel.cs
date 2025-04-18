using LMS.Entities.Models;

namespace LMS.Web.ViewModels;

public class ResourceItemViewModel
{
    public int ResourceId { get; set; }
    public string Title { get; set; }
    public ResourceType ResourceType { get; set; }
    public string SubjectName { get; set; }
    public string TeacherName { get; set; }
    public DateTime CreatedAt { get; set; }
}

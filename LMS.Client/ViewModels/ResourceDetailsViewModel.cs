using LMS.Entities.Models;

namespace LMS.Web.ViewModels;

public class ResourceDetailsViewModel
{
    public int ResourceId { get; set; }
    public string Title { get; set; }
    public ResourceType ResourceType { get; set; }
    public string ResourcePath { get; set; }
    public string? ResourceDescription { get; set; }
    public string SubjectName { get; set; }
    public string TeacherName { get; set; }
    public DateTime CreatedAt { get; set; }
}

namespace LMS.Web.ViewModels;

public class TeacherAssignmentViewModel
{
    public int AssignmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public string Status { get; set; } = string.Empty;
}

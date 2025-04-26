using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class NotificationToAllViewModel
{
    public NotificationType NotificationType { get; set; }

    [Required(ErrorMessage = "Message content is required")]
    public string Message { get; set; }

    public bool SendToStudents { get; set; }
    public bool SendToParents { get; set; }
    public bool SendToTeachers { get; set; }

    public List<int>? SelectedClassIds { get; set; } = new List<int>();
    public List<int>? SelectedSubjectIds { get; set; } = new List<int>();
    public List<int>? SelectedBusIds { get; set; } = new List<int>();
}

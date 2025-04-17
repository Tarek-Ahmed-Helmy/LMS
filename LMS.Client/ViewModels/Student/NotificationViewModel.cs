using LMS.Entities.Models;

namespace LMS.Web.ViewModels.StudentViewModels;

public class NotificationViewModel
{
    public int NotificationId { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationType NotificationType { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}

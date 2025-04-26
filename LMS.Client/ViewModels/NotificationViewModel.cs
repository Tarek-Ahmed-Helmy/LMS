using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class NotificationViewModel
{
    public int? NotificationId { get; set; }
    public string Message { get; set; } = string.Empty;

    [Display(Name = "Notification Type")]
    public NotificationType NotificationType { get; set; }
    [Display(Name = "Sender Name")]
    public string? SenderName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }

    [Display(Name = "Receiver Name")]
    public string? ReceiverName { get; set; }

    [Display(Name = "Receiver")]
    public string ReceiverId { get; set; }
}

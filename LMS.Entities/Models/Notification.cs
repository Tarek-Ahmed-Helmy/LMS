using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Entities.Models;

public class Notification
{
    public int NotificationId { get; set; }
    public bool IsRead { get; set; } = false;

    [Required]
    public required string Message { get; set; }

    [Required]
    [EnumDataType(typeof(NotificationType))]
    public NotificationType NotificationType { get; set; }

    [ForeignKey("Sender")]
    public required string SenderId { get; set; }

    [ForeignKey("Receiver")]
    public required string ReceiverId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ValidateNever]
    public ApplicationUser? Sender { get; set; }

    [ValidateNever]
    public ApplicationUser? Receiver { get; set; }
}

public enum NotificationType
{
    Alert,
    Reminder,
    Announcement
}
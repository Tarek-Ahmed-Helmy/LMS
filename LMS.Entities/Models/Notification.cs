using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Entities.Models;

public class Notification
{
    public int NotificationId { get; set; }

    [DefaultValue(false)]
    public bool IsRead { get; set; } = false;

    [Required]
    public required string Message { get; set; }

    [Required]
    public required string NotificationType { get; set; }

    [ForeignKey("Sender")]
    public string SenderId { get; set; }

    [ForeignKey("Receiver")]
    public string ReceiverId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ValidateNever]
    public ApplicationUser? Sender { get; set; }

    [ValidateNever]
    public ApplicationUser? Receiver { get; set; }
}

//NotificationType ('Alert', 'Reminder', 'Announcement')
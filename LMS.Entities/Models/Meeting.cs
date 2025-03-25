using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace LMS.Entities.Models;

public class Meeting
{
    public int MeetingId { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Please enter Title that its length more than 2 (Invalid Title)")]
    [MaxLength(500, ErrorMessage = "Please enter Title that its length less than 500 (Invalid Title)")]
    public required string Title { get; set; }

    [MinLength(2, ErrorMessage = "Please enter Notes that its length more than 2 (Invalid Notes)")]
    [MaxLength(2000, ErrorMessage = "Please enter Notes that its length less than 2000 (Invalid Notes)")]
    public string? Notes { get; set; }

    [Required]
    public required string MeetingLink { get; set; }
    public DateTime ScheduledDate { get; set; }

    [EnumDataType(typeof(MeetingStatus))]
    public MeetingStatus MeetingStatus { get; set; } = MeetingStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ValidateNever]
    public ICollection<Attende>? Attendees { get; set; }
}

public enum MeetingStatus
{
    Pending,
    Completed
}
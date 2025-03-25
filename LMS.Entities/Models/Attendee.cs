using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Entities.Models;

public class Attendee
{
    public int AttendeeId { get; set; }

    [Required]
    [ForeignKey("User")]
    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [ForeignKey("Meeting")]
    public int MeetingId { get; set; }
    public Meeting? Meeting { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Entities.Models;

public class Attende
{
    public int AttendeeId { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [ForeignKey("Meeting")]
    public int MeetingId { get; set; }
    public Meeting? Meeting { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
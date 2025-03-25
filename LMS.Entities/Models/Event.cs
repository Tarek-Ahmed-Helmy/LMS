using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LMS.Entities.Models;

public class Event
{
    public int EventId { get; set; }

    [MinLength(2, ErrorMessage = "Please enter title that its length more than 2 (invalid title)")]
    [MaxLength(500, ErrorMessage = "Please enter title that its length less than 500 (invalid title)")]
    [Required]
    public required string Title { get; set; }

    [MinLength(2, ErrorMessage = "Please enter Description that its length more than 2 ")]
    [MaxLength(2000, ErrorMessage = "Please enter Description that its length less than 2000 ")]
    [Required(ErrorMessage = "Please enter Description..!")]
    [DisplayName("Description")]
    public required string EventDescription { get; set; }

    [Required]
    public required string Location { get; set; }

    [Required]
    public DateTime EventDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
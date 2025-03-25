using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LMS.Entities.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [Display(Name = "Full Name")]
    public required string FullName { get; set; }

    [Required]
    public required string Address { get; set; }

    [Display(Name = "Profile Picture")]
    public string? ProfilePictureURL { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public ICollection<Meeting>? Meetings { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
}

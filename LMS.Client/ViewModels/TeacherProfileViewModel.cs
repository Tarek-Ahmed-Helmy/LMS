using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class TeacherProfileViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public required string FullName { get; set; }
    [Required]
    public required string Address { get; set; }
    [Display(Name = "Profile Picture")]
    public string? ProfilePictureURL { get; set; }
    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    [Required]
    public required string Qualification { get; set; }

    [Display(Name = "Experience (in years)")]
    public int Experience { get; set; }
}

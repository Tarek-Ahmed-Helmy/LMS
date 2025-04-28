using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class ProfileTeacherUpdateViewModel
{
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Display(Name = "Address")]
    public string Address { get; set; }

    [Display(Name = "Profile Picture")]
    public string ProfilePictureURL { get; set; }

    public IFormFile? ProfilePictureFile { get; set; }
    public DateTime HireDate { get; set; }
    public string Qualification { get; set; }
    public int Experience { get; set; } // In years
}

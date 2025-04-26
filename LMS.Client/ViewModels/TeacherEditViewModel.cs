using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class TeacherEditViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }

    [Required]
    public string Qualification { get; set; }

    [Required]
    public int Experience { get; set; } // In years
}

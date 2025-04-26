using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class ParentEditViewModel
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
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; set; }

    [Required]
    public string Occupation { get; set; }
}

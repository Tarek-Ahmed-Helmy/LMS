using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class EditUserViewModel
{
    //To avoid NullReferenceExceptions at runtime
    public EditUserViewModel()
    {
        Claims = new List<string>();
        Roles = new List<string>();
    }
    [Required]
    public string Id { get; set; }

    [Required]
    [Display(Name = "User Name")]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    public string Address { get; set; }
    public List<string> Claims { get; set; }
    public IList<string> Roles { get; set; }
}

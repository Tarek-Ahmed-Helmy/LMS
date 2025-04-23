using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Email Or Username")]
    public string EmailOrUserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class CreateUserViewModel
{
    [Required(ErrorMessage = "Full Name is Required")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Address is Required")]
    public string Address { get; set; }

    [Required]
    [EmailAddress]
    [Remote(action: "IsEmailAvailable", controller: "Account")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
    public List<SelectListItem>? Roles { get; set; }
    public List<string>? SelectedRoles { get; set; }
}

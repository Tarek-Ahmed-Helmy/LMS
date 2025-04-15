using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class CreateRoleViewModel
{
    [Required]
    [Display(Name = "Role")]
    public string RoleName { get; set; }
}

using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels.AdminViewModels
{
    public class ParentRegistrationViewModel
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
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Occupation { get; set; }
    }
}

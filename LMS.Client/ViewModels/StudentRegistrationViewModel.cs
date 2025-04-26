using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class StudentRegistrationViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public required string FullName { get; set; }
    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
    [Required]
    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }
    [Required]
    [StringLength(20)]
    public required string EmergencyContact { get; set; }
    [Required]
    public required string Address { get; set; }
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
    [Required]
    [DataType(DataType.Date)]
    public DateTime AdmissionDate { get; set; }
    [Required]
    [StringLength(255)]
    public required string StudentNumber { get; set; }

    [Display(Name = "Class Number")]
    public int ClassId { get; set; }
}

using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels.StudentViewModels;

public class StudentProfileViewModel
{
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required]
    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }
    [Required]
    [StringLength(20)]
    public required string EmergencyContact { get; set; }
    [Required]
    public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;
    [Required]
    [StringLength(255)]
    public required string StudentNumber { get; set; }
    [Required]
    [Display(Name = "Full Name")]
    public required string FullName { get; set; }
    [Required]
    public required string Address { get; set; }
    [Display(Name = "Profile Picture")]
    public string? ProfilePictureURL { get; set; }
    [Required]
    [EnumDataType(typeof(GradeLevel))]
    public GradeLevel GradeLevel { get; set; }
    [Required]
    [StringLength(255)]
    public required string ClassNumber { get; set; }
    [Display(Name = "Bus")]
    public int? BusId { get; set; }

}

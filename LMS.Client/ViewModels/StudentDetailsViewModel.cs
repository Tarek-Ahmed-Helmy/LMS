using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class StudentDetailsViewModel
{
    [Display(Name = "Full Name")]
    public string? FullName { get; set; }
    public string? Address { get; set; }

    [Display(Name = "Profile Picture")]
    public string? ProfilePictureURL { get; set; }
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }
    public string? EmergencyContact { get; set; }
    [DataType(DataType.Date)]
    public DateTime AdmissionDate { get; set; }
    [StringLength(255)]
    public string? StudentNumber { get; set; }
    [EnumDataType(typeof(GradeLevel))]
    public GradeLevel? GradeLevel { get; set; }
    [StringLength(255)]
    public string? ClassNumber { get; set; }
    public string? BusSubscription { get; set; }
}

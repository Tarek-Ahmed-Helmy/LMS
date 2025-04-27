using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class TeacherDetailsViewModel
{
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    public string Address { get; set; }

    [Display(Name = "Profile Picture")]
    public string ProfilePictureURL { get; set; }

    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }

    public string Qualification { get; set; }

    public int Experience { get; set; }  // In years
    public List<Schedule> Schedules { get; set; } = new List<Schedule>();
}

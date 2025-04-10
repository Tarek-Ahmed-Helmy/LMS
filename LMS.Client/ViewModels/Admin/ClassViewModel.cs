using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels.AdminViewModels;

public class ClassViewModel
{
    public int ClassId { get; set; }

    [EnumDataType(typeof(GradeLevel))]
    public GradeLevel GradeLevel { get; set; }

    public int Capacity { get; set; }
    public required string ClassNumber { get; set; }
}

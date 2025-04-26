using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class CreateBusViewModel
{
    public int? BusId { get; set; }
    [Required]
    [StringLength(255)]
    public string DriverName { get; set; } = string.Empty;

    [Required]
    public string Route { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
    public int Capacity { get; set; }

    [Required]
    [StringLength(20)]
    public string DriverContact { get; set; } = string.Empty;
}

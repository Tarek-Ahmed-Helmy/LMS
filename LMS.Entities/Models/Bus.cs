using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace LMS.Entities.Models;

public class Bus
{
    public int BusId { get; set; }

    [Required]
    [StringLength(255)]
    public required string DriverName { get; set; }

    [Required]
    public required string Route { get; set; }

    [Required]
    public int Capacity { get; set; }

    [Required]
    [StringLength(20)]
    public required string DriverContact { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public ICollection<Student>? Students { get; set; }
}
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Entities.Models;

public class Bus
{
    [Key]
    public int BusId { get; set; }

    [Required]
    [StringLength(255)]
    public string DriverName { get; set; }

    [Required]
    public string Route { get; set; }

    [Required]
    public int Capacity { get; set; }

    [Required]
    [StringLength(20)]
    public string DriverContact { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}


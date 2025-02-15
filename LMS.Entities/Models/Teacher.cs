using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Entities.Models;

public class Teacher
{
    [ForeignKey("ApplicationUser")]
    public string TeacherId { get; set; }
    public DateTime HireDate { get; set; } = DateTime.Now;
    public string Qualification { get; set; }
    public int Experience { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }
}

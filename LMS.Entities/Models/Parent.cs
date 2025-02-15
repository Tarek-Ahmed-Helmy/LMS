using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Entities.Models;

public class Parent
{
    [ForeignKey("ApplicationUser")]
    public string ParentId { get; set; }
    public string? Occupation { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; }
}

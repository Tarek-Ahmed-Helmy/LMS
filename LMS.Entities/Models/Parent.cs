using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Entities.Models;

public class Parent
{
    [ForeignKey("ApplicationUser")]
    public required string ParentId { get; set; }
    public string? Occupation { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; }

    [ValidateNever]
    public ICollection<Student>? Childrens { get; set; }

    [ValidateNever]
    public ICollection<Payment>? Payments { get; set; }
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Entities.Models;

public class Student
{
    [Key]
    [ForeignKey("ApplicationUser")]
    public string StudentId { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [StringLength(10)]
    [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either 'Male' or 'Female'.")]
    public string Gender { get; set; }

    [Required]
    public int GradeLevel { get; set; }

    [Required]
    [StringLength(20)]
    public string EmergencyContact { get; set; }

    [Required]
    public DateTime AdmissionDate { get; set; }

    [Required]
    [StringLength(255)]
    public string StudentNum { get; set; }

    [ForeignKey("Class")]
    public int? ClassId { get; set; }

    [ForeignKey("Parent")]
    public string? ParentId { get; set; }

    [ForeignKey("Bus")]
    public int? BusId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; }

    [ValidateNever]
    public Class? Class { get; set; }

    [ValidateNever]
    public Parent? Parent { get; set; }

    [ValidateNever]
    public Bus? Bus { get; set; }
}

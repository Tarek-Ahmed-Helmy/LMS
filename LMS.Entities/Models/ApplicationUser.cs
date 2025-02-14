using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Entities.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [Display(Name = "Full Name")]
    public required string FullName { get; set; }

    [Required]
    public required string Address { get; set; }

    [Display(Name = "Profile Picture")]
    public string? ProfilePictureURL { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

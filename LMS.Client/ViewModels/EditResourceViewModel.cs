using LMS.Entities.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class EditResourceViewModel
{
    public int ResourceId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? ResourceDescription { get; set; }

    [Required]
    public int SubjectId { get; set; }

    [Required]
    public ResourceType ResourceType { get; set; }

    public IFormFile? ResourceFile { get; set; } // Optional for editing
    public string? ExistingFilePath { get; set; } // To keep track of current file
}
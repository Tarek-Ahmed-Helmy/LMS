using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class CreateResourceViewModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? ResourceDescription { get; set; }

    [Required]
    public int SubjectId { get; set; }

    [Required]
    public ResourceType ResourceType { get; set; }

    [Required]
    public IFormFile ResourceFile { get; set; } = null!;
}

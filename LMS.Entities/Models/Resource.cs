using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LMS.Entities.Models;

 public class Resource
{
    public int ResourceId { get; set; }

    [Required, MaxLength(255)]
    public required string Title { get; set; }

    [Required, MaxLength(20)]
    [EnumDataType(typeof(ResourceType))]
    public ResourceType ResourceType { get; set; }

    [Required]
    [MaxLength(255)]
    [DisplayName("Attached File")]
    public required string ResourcePath { get; set; }

    [DisplayName("Description")]
    public string? ResourceDescription { get; set; }

    [Required]
    public int SubjectId { get; set; }

    [Required]
    public int TeacherId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ForeignKey("SubjectId")]
    public virtual Subject? Subject { get; set; }

    [ForeignKey("TeacherId")]
    public virtual Teacher? Teacher { get; set; }
}
public enum ResourceType
{
    Book,
    Video,
    Document,
    Link
}
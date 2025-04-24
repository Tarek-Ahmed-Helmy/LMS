using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

public class CreateAssignmentViewModel
{
    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required, DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    [Required, DataType(DataType.DateTime)]
    public DateTime Deadline { get; set; } = DateTime.UtcNow.AddDays(7);

    [Required, Range(1, 100)]
    public int TotalMarks { get; set; } = 10;

    [Required]
    public SubmissionType SubmissionType { get; set; } = SubmissionType.Online;

    [Required, Display(Name = "Subject")]
    public int SubjectId { get; set; }

   
    public IEnumerable<Subject>? Subjects { get; set; }
}

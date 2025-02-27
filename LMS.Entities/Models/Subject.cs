using System.ComponentModel;

namespace LMS.Entities.Models;

public class Subject
{
    public int SubjectId { get; set; }

    [DisplayName("Subject Name")]
    public string SubjectName { get; set; }

    [DisplayName("Code")]
    public string SubjectCode { get; set; }

    [DisplayName("Description")]
    public string SubjectDescription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now; 
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

}

//SubjectCode (Unique)
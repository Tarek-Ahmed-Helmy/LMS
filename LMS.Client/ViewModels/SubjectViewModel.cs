using System.ComponentModel;

namespace LMS.Web.ViewModels;

public class SubjectViewModel
{
    public int SubjectId { get; set; }

    [DisplayName("Subject Name")]
    public string SubjectName { get; set; }

    [DisplayName("Subject Code")]
    public string SubjectCode { get; set; }

    [DisplayName("Description")]
    public string SubjectDescription { get; set; }
}

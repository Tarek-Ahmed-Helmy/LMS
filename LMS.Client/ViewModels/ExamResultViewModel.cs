using LMS.Entities.Models;

namespace LMS.Web.ViewModels;

public class ExamResultViewModel
{
    public string Subject { get; set; } = null!;
    public ExamType ExamType { get; set; }
    public double Score { get; set; }
    public double TotalMarks { get; set; }
    public DateTime ExamDate { get; set; }
    public string Remarks { get; set; } = "";
}

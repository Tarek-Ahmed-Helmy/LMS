namespace LMS.Web.ViewModels;

public class StudentPaymentsViewModel
{
    public string FullName { get; set; } = null!;
    public string StudentId { get; set; } = null!;
    public List<PaymentRecordViewModel> Payments { get; set; } = new();
}

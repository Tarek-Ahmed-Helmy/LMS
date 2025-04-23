namespace LMS.Web.ViewModels;

public class PaymentRecordViewModel
{
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Method { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string ReceiptPath { get; set; } = null!;
}

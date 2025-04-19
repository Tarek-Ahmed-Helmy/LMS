using LMS.Entities.Models;

namespace LMS.Web.ViewModels;

public class FeeBreakdownViewModel
{
    public string StudentName { get; set; } = string.Empty;
    public GradeLevel Grade { get; set; }
    public decimal TotalFees { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount => TotalFees - PaidAmount;
    public DateTime DueDate { get; set; }
    public int PaymentId { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string ReceiptPath { get; set; } = string.Empty;
}

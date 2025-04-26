using LMS.Entities.Models;

namespace LMS.Web.ViewModels;

public class PaymentDetailsViewModel
{
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string ReceiptPath { get; set; } = "";
    public string StudentName { get; set; } = "";
    public string ParentName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
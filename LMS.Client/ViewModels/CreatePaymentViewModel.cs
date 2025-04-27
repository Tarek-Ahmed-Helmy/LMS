using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class CreatePaymentViewModel
{
    [Required]
    [Range(0.01, 1000000000)]
    public decimal Amount { get; set; }

    [Required]
    public string StudentId { get; set; } = "";
}

using LMS.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class MakePaymentViewModel
{
    [Required]
    public string StudentId { get; set; }
    public int PaymentId { get; set; }

    [Display(Name = "Student Name")]
    public string? StudentName { get; set; }

    public decimal? Amount { get; set; }

    [Required]
    [Display(Name = "Payment Method")]
    public PaymentMethod PaymentMethod { get; set; }

    [Required]
    public IFormFile Receipt { get; set; }
}


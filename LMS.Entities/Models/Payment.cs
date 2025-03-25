using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LMS.Entities.Models;

public class Payment
{
    public int PaymentId { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; }

    [Required, MaxLength(20)]
    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod PaymentMethod { get; set; }

    [Required, MaxLength(20)]
    [EnumDataType(typeof(PaymentStatus))]
    public PaymentStatus PaymentStatus { get; set; }

    [MaxLength(255)]
    [Required]
    [Display(Name = "Attached Receipt")]
    public required string ReceiptPath { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int ParentId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public virtual Parent? Parent { get; set; }

    [ValidateNever]
    public virtual Student? Student { get; set; }
}
public enum PaymentMethod
{
    Cash,
    Card,
    BankTransfer
}

public enum PaymentStatus
{
    Paid,
    Pending,
    Overdue
}
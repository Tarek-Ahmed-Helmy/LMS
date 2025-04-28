using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace LMS.Web.Areas.ParentArea.Controllers;

[Area("Parent")]
[Authorize(Roles = SD.ParentRole)]
public class FinanceController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FinanceController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public async Task<IActionResult> Fees()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var parent = await _unitOfWork.Parent.FindAsync(p => p.ParentId == id, ["Childrens.Payments", "Childrens.ApplicationUser"]);

        if (parent == null)
            return NotFound();

        var payments = parent.Childrens!
            .SelectMany(c => c.Payments!)
            .Select(p => new FeeBreakdownViewModel
            {
                PaymentId = p.PaymentId,
                StudentName = p.Student!.ApplicationUser.FullName,
                TotalFees = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod,
                PaymentStatus = p.PaymentStatus,
                ReceiptPath = p.ReceiptPath
            })
            .ToList();

        return View(payments);
    }

    [HttpGet]
    public async Task<IActionResult> MakePayment(int paymentId)
    {
        var payment = await _unitOfWork.Payment.FindAsync(p => p.PaymentId == paymentId, ["Student.ApplicationUser"]);
        var model = new MakePaymentViewModel
        {
            StudentId = payment.StudentId,
            StudentName = payment.Student!.ApplicationUser.FullName,
            Amount = payment.Amount,
            PaymentId = payment.PaymentId,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakePayment(int paymentId, MakePaymentViewModel model)
    {
        var payment = await _unitOfWork.Payment.FindAsync(p => p.PaymentId == paymentId, ["Student.ApplicationUser"]);

        if (!ModelState.IsValid)
        {
            var vm = new MakePaymentViewModel
            {
                StudentId = payment.StudentId,
                StudentName = payment.Student!.ApplicationUser.FullName,
                Amount = payment.Amount,
                PaymentId = payment.PaymentId,
            };
            return View(vm);
        }

        var receiptFileName = $"{Guid.NewGuid()}_{model.Receipt.FileName}";
        var receiptPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "receipts", receiptFileName);

        using (var stream = new FileStream(receiptPath, FileMode.Create))
        {
            await model.Receipt.CopyToAsync(stream);
        }

        payment.PaymentMethod = model.PaymentMethod;
        payment.PaymentStatus = PaymentStatus.Paid;
        payment.ReceiptPath = $"/uploads/receipts/{receiptFileName}";
        payment.PaymentDate = DateTime.UtcNow;

        await _unitOfWork.Payment.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction("Fees");
    }

    [HttpGet]
    public async Task<IActionResult> DownloadReceipt(int paymentId)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var payment = await _unitOfWork.Payment.FindAsync(p => p.PaymentId == paymentId);

        if (payment == null)
            return NotFound("Payment not found or access denied.");

        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, payment.ReceiptPath.TrimStart('/'));

        if (!System.IO.File.Exists(filePath))
            return NotFound("Receipt file not found.");

        var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
        var contentType = fileExtension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        var fileName = $"Receipt_{payment.PaymentId}{fileExtension}";

        return File(fileBytes, contentType, fileName);
    }

}

using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace LMS.Web.Areas.ParentArea;

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
    public async Task<IActionResult> MakePayment()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var studentList = (await _unitOfWork.Student.FindAllAsync(s => s.ParentId == id, ["ApplicationUser"]))
            .Select(s => new SelectListItem
            {
                Value = s.StudentId,
                Text = s.ApplicationUser.FullName
            });

        ViewBag.Students = studentList;

        return View(new MakePaymentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakePayment(MakePaymentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.Students = (await _unitOfWork.Student.FindAllAsync(s => s.ParentId == id, ["ApplicationUser"]))
                .Select(s => new SelectListItem
                {
                    Value = s.StudentId,
                    Text = s.ApplicationUser.FullName
                });
            return View(model);
        }

        var receiptFileName = $"{Guid.NewGuid()}_{model.Receipt.FileName}";
        var receiptPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "receipts", receiptFileName);

        using (var stream = new FileStream(receiptPath, FileMode.Create))
        {
            await model.Receipt.CopyToAsync(stream);
        }

        var payment = new Payment
        {
            StudentId = model.StudentId,
            ParentId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Amount = model.Amount,
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = model.PaymentMethod,
            PaymentStatus = PaymentStatus.Paid,
            ReceiptPath = "/uploads/receipts/" + receiptFileName
        };

        await _unitOfWork.Payment.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        TempData["Success"] = "Payment successful.";
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

using System.Security.Claims;
using LMS.DataAccess.Repositories;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> GetPaymentList()
        {
            var payments = await _unitOfWork.Payment.FindAllAsync(
                includes: new[] { "Student.ApplicationUser", "Parent.ApplicationUser" });

            var data = payments.Select(p => new {
                p.PaymentId,
                p.Amount,
                PaymentDate = p.PaymentDate,
                Method = p.PaymentMethod.ToString(),
                Status = p.PaymentStatus.ToString(),
                StudentName = p.Student!.ApplicationUser!.FullName,
                ParentName = p.Parent!.ApplicationUser!.FullName
            }).ToList();

            return Json(new { data });
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var p = await _unitOfWork.Payment.FindAsync(
                x => x.PaymentId == id,
                new[] { "Student.ApplicationUser", "Parent.ApplicationUser" });
            if (p is null) return NotFound();

            var vm = new PaymentDetailsViewModel
            {
                PaymentId = p.PaymentId,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                Method = p.PaymentMethod,
                Status = p.PaymentStatus,
                ReceiptPath = p.ReceiptPath,
                StudentName = p.Student!.ApplicationUser!.FullName,
                ParentName = p.Parent!.ApplicationUser!.FullName,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {

            await PopulatePage();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePaymentViewModel vm)
        {

            if (!ModelState.IsValid)
            {

                await PopulatePage();

                return View(vm);
            }
                
            var student = await _unitOfWork.Student.FindAsync(x => x.StudentId == vm.StudentId);
            if (student.ParentId is null)
            {
                ModelState.AddModelError("", "This Student Has No Parents");
                await PopulatePage();
                return View(vm);
            }

            var payment = new Payment
            {
                StudentId = vm.StudentId,
                ParentId = student.ParentId,
                Amount = vm.Amount,
                PaymentStatus = PaymentStatus.Pending,
                ReceiptPath = string.Empty 
            };

            await _unitOfWork.Payment.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _unitOfWork.Payment.FindAsync(x => x.PaymentId == id);
            if (p is null) return NotFound();

            var vm = new EditPaymentViewModel
            {
                PaymentId = p.PaymentId,
                Amount = p.Amount,
                StudentId = p.StudentId
            };

            await PopulatePage();
            return View(vm);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPaymentViewModel vm)
        {
            vm.Students = await _unitOfWork.Student.GetAllAsync();
            vm.Parents = await _unitOfWork.Parent.GetAllAsync();

            if (!ModelState.IsValid)

            {
                await PopulatePage();

                return View(vm);
            } 

            var p = await _unitOfWork.Payment.FindAsync(x => x.PaymentId == id);
            if (p is null) return NotFound();

            p.Amount = vm.Amount;
         

            p.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Payment.UpdateAsync(p);  
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task PopulatePage()
        {
            var students = await _unitOfWork.Student.FindAllAsync(includes: new[] { "ApplicationUser" });
            ViewBag.Students = students
                .OrderBy(s => s.StudentNumber)
                .ThenBy(s => s.ApplicationUser!.FullName)
                .Select(s => new SelectListItem
                {
                    Value = s.StudentId,
                    Text = $"{s.StudentNumber} - {s.ApplicationUser.FullName}"
                })
                .ToList();


            var parents = await _unitOfWork.Parent.FindAllAsync(includes: new[] { "ApplicationUser" }
            );
            ViewBag.Parents = parents
                .OrderBy(p => p.ApplicationUser!.FullName)
                .Select(p => new SelectListItem
                {
                    Value = p.ParentId,
                    Text = p.ApplicationUser.FullName
                })
                .ToList();

        }
    }
}

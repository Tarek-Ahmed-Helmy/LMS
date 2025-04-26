using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Web.Areas.AdminArea.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.AdminRole)]
public class StudentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    public StudentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<JsonResult> GetStudentList()
    {
        var students = await _unitOfWork.Student.FindAllAsync(includes: ["ApplicationUser", "Class"]);
        var studentList = students.Select(s => new
        {
            s.StudentId,
            s.StudentNumber,
            s.ApplicationUser?.FullName,
            s.ApplicationUser?.Email,
            s.DateOfBirth,
            s.Gender,
            s.AdmissionDate,
            s.Class?.GradeLevel,
            s.Class?.ClassNumber,
            IsLocked = s.ApplicationUser?.LockoutEnd != null && s.ApplicationUser.LockoutEnd > DateTimeOffset.UtcNow ? "Locked" : "Active"

        }).ToList();
        return Json(new {data = studentList});
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, includes: new string[] { "ApplicationUser", "Class"});
        if (student == null) return NotFound();
        var model = new StudentDetailsViewModel
        {
            FullName = student.ApplicationUser?.FullName,
            Address = student.ApplicationUser?.Address,
            ProfilePictureURL = student.ApplicationUser?.ProfilePictureURL,
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            EmergencyContact = student.EmergencyContact,
            AdmissionDate = student.AdmissionDate,
            StudentNumber = student.StudentNumber,
            GradeLevel = student.Class?.GradeLevel,
            ClassNumber = student.Class?.ClassNumber,
            BusSubscription = student.BusId != null ? "Subscriped" : "Unsubscriped"
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var classes = await _unitOfWork.Class.GetAllAsync();
        ViewBag.Classes = classes
            .OrderBy(c => c.GradeLevel)
            .ThenBy(c => c.ClassNumber)
            .Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = $"{c.GradeLevel} - {c.ClassNumber}"
            }).ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentRegistrationViewModel newStudent)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = newStudent.Email,
                Email = newStudent.Email,
                FullName = newStudent.FullName,
                Address = newStudent.Address
            };

            var result = await _userManager.CreateAsync(user, newStudent.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.StudentRole);
                var student = new Student
                {
                    StudentId = user.Id,
                    EmergencyContact = newStudent.EmergencyContact,
                    DateOfBirth = newStudent.DateOfBirth,
                    AdmissionDate = newStudent.AdmissionDate,
                    Gender = newStudent.Gender,
                    StudentNumber = newStudent.StudentNumber,
                    ClassId = newStudent.ClassId
                };
                await _unitOfWork.Student.AddAsync(student);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(newStudent);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        //need to add another fields to the view model to be updated
        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, includes: new string[] { "ApplicationUser" });
        if (student == null) return NotFound();

        var classes = await _unitOfWork.Class.GetAllAsync();
        ViewBag.Classes = classes
            .OrderBy(c => c.GradeLevel)
            .ThenBy(c => c.ClassNumber)
            .Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = $"{c.GradeLevel} - {c.ClassNumber}"
            }).ToList();

        var buses = await _unitOfWork.Bus.GetAllAsync();
        ViewBag.Buses = buses
            .Select(b => new SelectListItem
            {
                Value = b.BusId.ToString(),
                Text = $"{b.BusId} - {b.Route}"
            }).ToList();

        var viewModel = new StudentEditViewModel
        {
            FullName = student.ApplicationUser.FullName,
            Email = student.ApplicationUser.Email,
            Address = student.ApplicationUser.Address,
            DateOfBirth = student.DateOfBirth,
            AdmissionDate = student.AdmissionDate,
            EmergencyContact = student.EmergencyContact,
            Gender = student.Gender,
            StudentNumber = student.StudentNumber,
            ClassId = student.ClassId,
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, StudentEditViewModel updatedStudent)
    {
        if (!ModelState.IsValid)
            return View(updatedStudent);

        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, includes: new string[] { "ApplicationUser" });
        if (student == null) return NotFound();

        // Update ApplicationUser
        student.ApplicationUser.FullName = updatedStudent.FullName;
        student.ApplicationUser.Email = updatedStudent.Email;
        student.ApplicationUser.UserName = updatedStudent.Email;
        student.ApplicationUser.Address = updatedStudent.Address;

        // Update Student
        student.DateOfBirth = updatedStudent.DateOfBirth;
        student.AdmissionDate = updatedStudent.AdmissionDate;
        student.EmergencyContact = updatedStudent.EmergencyContact;
        student.Gender = updatedStudent.Gender;
        student.StudentNumber = updatedStudent.StudentNumber;
        student.ClassId = updatedStudent.ClassId;
        student.BusId = updatedStudent.BusId;

        await _unitOfWork.ApplicationUser.UpdateAsync(student.ApplicationUser);
        await _unitOfWork.Student.UpdateAsync(student);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ChangeStatus(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (!user.LockoutEnabled)
        {
            user.LockoutEnabled = true;
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            TempData["StatusMessage"] = "Student account has been unlocked.";
        }
        else
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            TempData["StatusMessage"] = "Student account has been locked.";
        }
        return RedirectToAction(nameof(Index));
    }
}

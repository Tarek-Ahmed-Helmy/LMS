using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.AdminArea.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.AdminRole)]
public class TeacherController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public TeacherController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    // GET: /Teacher/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var teachers = await _unitOfWork.Teacher.FindAllAsync(includes: new string[] { "ApplicationUser" });
        return View(teachers);
    }

    // GET: /Teacher/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var teacher = await _unitOfWork.Teacher.FindAsync(t => t.TeacherId == id, includes: new string[] { "ApplicationUser" });
        if (teacher == null) return NotFound();

        var model = new TeacherDetailsViewModel
        {
            FullName = teacher.ApplicationUser?.FullName,
            Address = teacher.ApplicationUser?.Address,
            ProfilePictureURL = teacher.ApplicationUser?.ProfilePictureURL,
            HireDate = teacher.HireDate,
            Qualification = teacher.Qualification,
            Experience = teacher.Experience
        };

        return View(model);
    }

    // GET: /Teacher/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Teacher/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeacherRegistrationViewModel newTeacher)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = newTeacher.Email,
                Email = newTeacher.Email,
                FullName = newTeacher.FullName,
                Address = newTeacher.Address
            };

            var result = await _userManager.CreateAsync(user, newTeacher.Password);
            if (result.Succeeded)
            {
                // Add the Teacher role to the user later (handled after role system setup)
                var teacher = new Teacher
                {
                    TeacherId = user.Id,
                    HireDate = newTeacher.HireDate,
                    Qualification = newTeacher.Qualification,
                    Experience = newTeacher.Experience
                };
                await _unitOfWork.Teacher.AddAsync(teacher);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(newTeacher);
    }

    // GET: /Teacher/Edit/{id}
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var teacher = await _unitOfWork.Teacher.FindAsync(t => t.TeacherId == id, includes: new string[] { "ApplicationUser" });
        if (teacher == null) return NotFound();

        var viewModel = new TeacherRegistrationViewModel
        {
            FullName = teacher.ApplicationUser.FullName,
            Email = teacher.ApplicationUser.Email,
            Address = teacher.ApplicationUser.Address,
            HireDate = teacher.HireDate,
            Qualification = teacher.Qualification,
            Experience = teacher.Experience
        };
        return View(viewModel);
    }

    // POST: /Teacher/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, TeacherRegistrationViewModel updatedTeacher)
    {
        if (!ModelState.IsValid)
            return View(updatedTeacher);

        var teacher = await _unitOfWork.Teacher.FindAsync(t => t.TeacherId == id, includes: new string[] { "ApplicationUser" });
        if (teacher == null) return NotFound();

        // Update ApplicationUser
        teacher.ApplicationUser.FullName = updatedTeacher.FullName;
        teacher.ApplicationUser.Email = updatedTeacher.Email;
        teacher.ApplicationUser.UserName = updatedTeacher.Email;
        teacher.ApplicationUser.Address = updatedTeacher.Address;

        // Update Teacher
        teacher.HireDate = updatedTeacher.HireDate;
        teacher.Qualification = updatedTeacher.Qualification;
        teacher.Experience = updatedTeacher.Experience;

        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Teacher/ChangeStatus/{id}
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
            TempData["StatusMessage"] = "Teacher account has been unlocked.";
        }
        else
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            TempData["StatusMessage"] = "Teacher account has been locked.";
        }

        return RedirectToAction(nameof(Index));
    }
}


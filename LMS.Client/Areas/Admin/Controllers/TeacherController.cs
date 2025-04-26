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
public class TeacherController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public TeacherController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    // GET: /Admin/Teacher
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // GET: /Admin/Teacher/GetTeacherList
    [HttpGet]
    public async Task<JsonResult> GetTeacherList()
    {
        var teachers = await _unitOfWork.Teacher.FindAllAsync(includes: new[] { "ApplicationUser" });
        var data = teachers.Select(t => new
        {
            t.TeacherId,
            FullName = t.ApplicationUser?.FullName,
            Email = t.ApplicationUser?.Email,
            HireDate = t.HireDate,
            Qualification = t.Qualification,
            Experience = t.Experience,
            IsLocked = (t.ApplicationUser?.LockoutEnd != null && t.ApplicationUser.LockoutEnd > DateTimeOffset.UtcNow) ? "Locked" : "Active"
        }).ToList();

        return Json(new { data });
    }
    // GET: /Teacher/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var teacher = await _unitOfWork.Teacher.FindAsync(t => t.TeacherId == id, includes: new string[] { "ApplicationUser", "Schedules.Class", "Schedules.Subject" });
        if (teacher == null) return NotFound();

        var model = new TeacherDetailsViewModel
        {
            FullName = teacher.ApplicationUser?.FullName,
            Address = teacher.ApplicationUser?.Address,
            ProfilePictureURL = teacher.ApplicationUser?.ProfilePictureURL,
            HireDate = teacher.HireDate,
            Qualification = teacher.Qualification,
            Experience = teacher.Experience,
            Schedules = teacher.Schedules.ToList() ?? new List<Schedule>()
        };

        ViewBag.TeacherId = id;

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
                await _userManager.AddToRoleAsync(user, SD.TeacherRole);
                var teacher = new LMS.Entities.Models.Teacher
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

    [HttpGet]
    public async Task<IActionResult> AddSchedule(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var teacher = await _unitOfWork.Teacher.GetByIdAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }

        await PopulateViewBags();

        var model = new AddScheduleViewModel
        {
            TeacherId = teacher.TeacherId
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSchedule(AddScheduleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateViewBags();
            return View(model);
        }

        var teacher = await _unitOfWork.Teacher.GetByIdAsync(model.TeacherId);
        if (teacher == null)
        {
            return NotFound();
        }

        var schedule = new Schedule
        {
            TeacherId = model.TeacherId,
            ClassId = model.ClassId,
            SubjectId = model.SubjectId,
            DayOfWeek = model.DayOfWeek, 
            StartTime = model.StartTime,
            EndTime = model.EndTime
        };

        await _unitOfWork.Schedule.AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = model.TeacherId });
    }


    private async Task PopulateViewBags()
    {
        var subjects = await _unitOfWork.Subject.GetAllAsync();
        ViewBag.Subject = subjects
            .OrderBy(s => s.SubjectCode)
            .ThenBy(s => s.SubjectName)
            .Select(s => new SelectListItem
            {
                Value = s.SubjectId.ToString(),
                Text = $"{s.SubjectCode} - {s.SubjectName}"
            }).ToList();

        var classes = await _unitOfWork.Class.GetAllAsync();
        ViewBag.Class = classes
            .OrderBy(c => c.GradeLevel)
            .ThenBy(c => c.ClassNumber)
            .Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = $"{c.GradeLevel} - {c.ClassNumber}"
            }).ToList();
    }

    [HttpGet]
    public async Task<IActionResult> EditSchedule(int id, string teacherId)
    {
        var schedule = await _unitOfWork.Schedule.GetByIdAsync(id);
        if (schedule == null)
        {
            return NotFound();
        }

        await PopulateViewBags();

        var model = new AddScheduleViewModel
        {
            TeacherId = teacherId,
            ScheduleId = schedule.ScheduleId,
            ClassId = schedule.ClassId,
            SubjectId = schedule.SubjectId,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSchedule(AddScheduleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateViewBags();
            return View(model);
        }

        var teacher = await _unitOfWork.Teacher.GetByIdAsync(model.TeacherId);
        var schedule = await _unitOfWork.Schedule.GetByIdAsync(model.ScheduleId.Value);
        if (teacher == null || schedule == null)
        {
            return NotFound();
        }

        schedule.ClassId = model.ClassId;
        schedule.SubjectId = model.SubjectId;
        schedule.DayOfWeek = model.DayOfWeek;
        schedule.StartTime = model.StartTime;
        schedule.EndTime = model.EndTime;

        await _unitOfWork.Schedule.UpdateAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = model.TeacherId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSchedule(int id, string teacherId)
    {
        var schedule = await _unitOfWork.Schedule.GetByIdAsync(id);
        if (schedule == null)
        {
            return NotFound();
        }
        await _unitOfWork.Schedule.DeleteAsync(schedule);
        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = teacherId });
    }
}


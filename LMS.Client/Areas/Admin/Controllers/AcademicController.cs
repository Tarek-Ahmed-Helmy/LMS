using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.AdminArea.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.AdminRole)]
public class AcademicController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AcademicController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult Subjects()
    {
        return View();
    }

    // GET: /Admin/Academic/GetSubjectList
    [HttpGet]
    public async Task<JsonResult> GetSubjectList()
    {
        var subjects = await _unitOfWork.Subject.GetAllAsync();
        var data = subjects.Select(s => new
        {
            s.SubjectId,
            s.SubjectCode,
            s.SubjectName,
            s.SubjectDescription
            
        }).ToList();
        return Json(new { data });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSubject(SubjectViewModel newSubject)
    {
        if (ModelState.IsValid)
        {
            var subject = new Subject
            {
                SubjectName = newSubject.SubjectName,
                SubjectCode = newSubject.SubjectCode,
                SubjectDescription = newSubject.SubjectDescription
            };

            await _unitOfWork.Subject.AddAsync(subject);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Subjects));
        }
        return RedirectToAction(nameof(Subjects));
    }

    [HttpGet]
    public async Task<IActionResult> UpdateSubject(int id)
    {
        var subject = await _unitOfWork.Subject.GetByIdAsync(id);
        if (subject == null) return NotFound();

        var viewModel = new SubjectViewModel
        {
            SubjectName = subject.SubjectName,
            SubjectCode = subject.SubjectCode,
            SubjectDescription = subject.SubjectDescription
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSubject(int id, SubjectViewModel model)
    {
        var subject = await _unitOfWork.Subject.GetByIdAsync(id);
        if (subject == null) return NotFound();

        subject.SubjectName = model.SubjectName;
        subject.SubjectCode = model.SubjectCode;
        subject.SubjectDescription = model.SubjectDescription;

        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Subjects));
    }


    [HttpGet]
    public IActionResult Classes()
    {
        return View();
    }

    // GET: /Admin/Academic/GetClassList
    [HttpGet]
    public async Task<JsonResult> GetClassList()
    {
        var classes = await _unitOfWork.Class.GetAllAsync();
        var data = classes.Select(c => new
        {
            c.ClassId,
            c.ClassNumber,
            c.GradeLevel,
            c.Capacity
        }).ToList();
        return Json(new { data });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddClass(ClassManagementViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var newClass = new Class
            {
                GradeLevel = viewModel.NewClass.GradeLevel,
                Capacity = viewModel.NewClass.Capacity,
                ClassNumber = viewModel.NewClass.ClassNumber
            };

            await _unitOfWork.Class.AddAsync(newClass);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Classes));
        }
        viewModel.Classes = await _unitOfWork.Class.GetAllAsync();
        return View("Classes", viewModel);

    }

    [HttpGet]
    public async Task<IActionResult> UpdateClass(int id)
    {
        var classEntity = await _unitOfWork.Class.GetByIdAsync(id);
        if (classEntity == null) return NotFound();

        var viewModel = new ClassViewModel
        {
            GradeLevel = classEntity.GradeLevel,
            Capacity = classEntity.Capacity,
            ClassNumber = classEntity.ClassNumber
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateClass(int id, ClassViewModel model)
    {
        var classEntity = await _unitOfWork.Class.GetByIdAsync(id);
        if (classEntity == null) return NotFound();

        classEntity.Capacity = model.Capacity;
        classEntity.ClassNumber = model.ClassNumber;
        classEntity.GradeLevel = model.GradeLevel;

        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Classes));
    }
}

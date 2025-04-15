using LMS.Entities.Interfaces;
using LMS.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.AdminArea.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.AdminRole)]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public DashboardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalStudents = await _unitOfWork.Student.CountAsync();
        ViewBag.TotalTeachers = await _unitOfWork.Teacher.CountAsync();
        ViewBag.TotalParents = await _unitOfWork.Parent.CountAsync();
        ViewBag.TotalClasses = await _unitOfWork.Class.CountAsync();
        ViewBag.TotalSubjects = await _unitOfWork.Subject.CountAsync();
        ViewBag.TotalEvents = await _unitOfWork.Event.CountAsync();
        return View();
    }
}

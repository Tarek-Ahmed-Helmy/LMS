using LMS.Entities.Interfaces;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Areas.StudentArea.Controllers;

[Area("Student")]
[Authorize(Roles =SD.StudentRole)]
public class ResourceController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ResourceController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, ["Class.Schedules"]);

        if (student is null || student.Class is null || student.Class.Schedules is null) return NotFound();

        var resources = await _unitOfWork.Resource.FindAllAsync(r => student.Class.Schedules.Select(s => s.SubjectId).Contains(r.SubjectId), ["Subject", "Teacher.ApplicationUser"]);

        var viewModel = resources.Select(r => new ResourceItemViewModel
        {
            ResourceId = r.ResourceId,
            Title = r.Title,
            ResourceType = r.ResourceType,
            SubjectName = r.Subject?.SubjectName ?? "N/A",
            TeacherName = r.Teacher?.ApplicationUser?.FullName ?? "N/A",
            CreatedAt = r.CreatedAt
        }).ToList();

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int resourceId)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var resource = await _unitOfWork.Resource.FindAsync(r => r.ResourceId == resourceId, ["Subject", "Teacher.ApplicationUser"]);

        if (resource is null)
            return NotFound();

        var viewModel = new ResourceDetailsViewModel
        {
            ResourceId = resource.ResourceId,
            Title = resource.Title,
            ResourceType = resource.ResourceType,
            ResourcePath = resource.ResourcePath,
            ResourceDescription = resource.ResourceDescription,
            SubjectName = resource.Subject?.SubjectName ?? "N/A",
            TeacherName = resource.Teacher?.ApplicationUser?.FullName ?? "N/A",
            CreatedAt = resource.CreatedAt
        };

        return View(viewModel);
    }
}

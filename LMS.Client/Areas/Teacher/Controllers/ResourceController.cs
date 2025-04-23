using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Areas.TeacherArea.Controllers;

[Area("Teacher")]
[Authorize(Roles = SD.TeacherRole)]
public class ResourceController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ResourceController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: /Teacher/Resource/Index
    public async Task<IActionResult> Index()
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var resources = await _unitOfWork.Resource.FindAllAsync(
            r => r.TeacherId == teacherId,
            includes: ["Subject"]
        );

        var viewModel = resources.Select(r => new TeacherResourceItemViewModel
        {
            ResourceId = r.ResourceId,
            Title = r.Title,
            SubjectName = r.Subject?.SubjectName ?? "N/A",
            CreatedAt = r.CreatedAt,
            ResourceType = r.ResourceType
        }).ToList();

        return View(viewModel);
    }

    // GET: /Teacher/Resource/Create
    public async Task<IActionResult> Create()
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // Fetch subjects taught by the teacher via Schedule
        var schedules = await _unitOfWork.Schedule.FindAllAsync(
            s => s.TeacherId == teacherId,
            includes: ["Subject"]
        );

        var subjects = schedules
            .Where(s => s.Subject != null)
            .Select(s => s.Subject!)
            .DistinctBy(s => s.SubjectId)
            .ToList();

        ViewBag.Subjects = subjects;
        ViewBag.ResourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
        return View(new CreateResourceViewModel());
    }

    // POST: /Teacher/Resource/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateResourceViewModel model)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!ModelState.IsValid)
        {
            var schedules = await _unitOfWork.Schedule.FindAllAsync(s => s.TeacherId == teacherId, includes: ["Subject"]);
            ViewBag.Subjects = schedules.Where(s => s.Subject != null).Select(s => s.Subject!).DistinctBy(s => s.SubjectId).ToList();
            ViewBag.ResourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
            return View(model);
        }

        var fileName = $"{Guid.NewGuid()}_{model.ResourceFile.FileName}";
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/teacher-resources", fileName);

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await model.ResourceFile.CopyToAsync(stream);
        }

        var resource = new Resource
        {
            Title = model.Title,
            ResourceDescription = model.ResourceDescription,
            ResourcePath = $"/uploads/teacher-resources/{fileName}",
            ResourceType = model.ResourceType,
            SubjectId = model.SubjectId,
            TeacherId = teacherId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Resource.AddAsync(resource);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Teacher/Resource/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var resource = await _unitOfWork.Resource.FindAsync(r => r.ResourceId == id, includes: ["Subject"]);
        if (resource == null) return NotFound();

        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var schedules = await _unitOfWork.Schedule.FindAllAsync(
            s => s.TeacherId == teacherId,
            includes: ["Subject"]
        );

        var subjects = schedules
            .Where(s => s.Subject != null)
            .Select(s => s.Subject!)
            .DistinctBy(s => s.SubjectId)
            .ToList();

        var model = new EditResourceViewModel
        {
            ResourceId = resource.ResourceId,
            Title = resource.Title,
            ResourceDescription = resource.ResourceDescription,
            SubjectId = resource.SubjectId,
            ResourceType = resource.ResourceType,
            ExistingFilePath = resource.ResourcePath
        };

        ViewBag.Subjects = subjects;
        ViewBag.ResourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
        return View(model);
    }

    // POST: /Teacher/Resource/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditResourceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var schedules = await _unitOfWork.Schedule.FindAllAsync(s => s.TeacherId == teacherId, includes: ["Subject"]);
            ViewBag.Subjects = schedules.Where(s => s.Subject != null).Select(s => s.Subject!).DistinctBy(s => s.SubjectId).ToList();
            ViewBag.ResourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
            return View(model);
        }

        var resource = await _unitOfWork.Resource.FindAsync(r => r.ResourceId == model.ResourceId);
        if (resource == null) return NotFound();

        resource.Title = model.Title;
        resource.ResourceDescription = model.ResourceDescription;
        resource.SubjectId = model.SubjectId;
        resource.ResourceType = model.ResourceType;
        resource.UpdatedAt = DateTime.UtcNow;

        if (model.ResourceFile != null && model.ResourceFile.Length > 0)
        {
            // Delete old file if exists
            if (!string.IsNullOrEmpty(model.ExistingFilePath))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, model.ExistingFilePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            var fileName = $"{Guid.NewGuid()}_{model.ResourceFile.FileName}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/teacher-resources", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ResourceFile.CopyToAsync(stream);
            }

            resource.ResourcePath = $"/uploads/teacher-resources/{fileName}";
        }

        await _unitOfWork.Resource.UpdateAsync(resource);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST: /Teacher/Resource/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var resource = await _unitOfWork.Resource.FindAsync(r => r.ResourceId == id);
        if (resource == null) return NotFound();

        // Delete file from disk
        if (!string.IsNullOrEmpty(resource.ResourcePath))
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, resource.ResourcePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        await _unitOfWork.Resource.DeleteAsync(resource);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    // GET: /Teacher/Resource/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var resource = await _unitOfWork.Resource.FindAsync(
            r => r.ResourceId == id,
            includes: ["Subject"]
        );

        if (resource == null)
            return NotFound();

        var viewModel = new ResourceDetailsViewModel
        {
            ResourceId = resource.ResourceId,
            Title = resource.Title,
            ResourceDescription = resource.ResourceDescription,
            SubjectName = resource.Subject?.SubjectName ?? "N/A",
            ResourceType = resource.ResourceType,
            ResourcePath = resource.ResourcePath,
            CreatedAt = resource.CreatedAt
        };

        return View(viewModel);
    }

}
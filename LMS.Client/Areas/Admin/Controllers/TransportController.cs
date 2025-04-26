using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.AdminRole)]
public class TransportController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public TransportController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: /Admin/Transport
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // GET: /Admin/Transport/GetBusList
    [HttpGet]
    public async Task<JsonResult> GetBusList()
    {
        var buses = await _unitOfWork.Bus.GetAllAsync();
        var data = buses.Select(b => new
        {
            b.BusId,
            b.DriverName,
            b.Route,
            b.Capacity,
            b.DriverContact,
            StudentCount = b.Students?.Count ?? 0
        }).ToList();

        return Json(new { data });
    }


    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var bus = await _unitOfWork.Bus.FindAsync(b => b.BusId == id, ["Students.ApplicationUser", "Students.Class"]);
        if (bus == null)
        {
            return NotFound();
        }

        var allStudents = await _unitOfWork.Student.FindAllAsync(s=>s.BusId == null, ["ApplicationUser"]);
        ViewBag.Students = allStudents
            .OrderBy(s => s.StudentNumber)
            .ThenBy(s => s.ApplicationUser.FullName)
            .Select(s => new SelectListItem
            {
                Value = s.StudentId.ToString(),
                Text = $"{s.StudentNumber} - {s.ApplicationUser.FullName}"
            }).ToList();

        var viewModel = new BusDetailsViewModel
        {
            BusId = bus.BusId,
            DriverName = bus.DriverName,
            Route = bus.Route,
            Capacity = bus.Capacity,
            DriverContact = bus.DriverContact,
            StudentCount = bus.Students?.Count ?? 0,
            Students = bus.Students?.Select(s => new StudentBusViewModel
            {
                Id = s.StudentId,
                FullName = s.ApplicationUser?.FullName ?? "Unknown",
                StudentNumber = s.StudentNumber,
                ClassNumber = s.Class?.ClassNumber ?? string.Empty,
                GradeLevel = s.Class.GradeLevel,
                EmergencyContact = s.EmergencyContact
            }).ToList() ?? new List<StudentBusViewModel>()
        };
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBusViewModel model)
    {
        if (ModelState.IsValid)
        {
            var bus = new Bus
            {
                DriverName = model.DriverName,
                Route = model.Route,
                Capacity = model.Capacity,
                DriverContact = model.DriverContact
            };
            await _unitOfWork.Bus.AddAsync(bus);
            await _unitOfWork.SaveChangesAsync();
            TempData["StatusMessage"] = "Bus added successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var bus = await _unitOfWork.Bus.FindAsync(b => b.BusId == id);
        if (bus == null)
        {
            return NotFound();
        }
        var model = new CreateBusViewModel
        {
            DriverName = bus.DriverName,
            Route = bus.Route,
            Capacity = bus.Capacity,
            DriverContact = bus.DriverContact
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateBusViewModel model)
    {
        if (ModelState.IsValid)
        {
            var bus = await _unitOfWork.Bus.FindAsync(b => b.BusId == id);
            if (bus == null)
            {
                return NotFound();
            }
            bus.DriverName = model.DriverName;
            bus.Route = model.Route;
            bus.Capacity = model.Capacity;
            bus.DriverContact = model.DriverContact;

            await _unitOfWork.Bus.UpdateAsync(bus);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var bus = await _unitOfWork.Bus.FindAsync(b => b.BusId == id);
        if (bus == null)
        {
            return NotFound();
        }
        await _unitOfWork.Bus.DeleteAsync(bus);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromBus(string studentId, int busId)
    {
        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == studentId);
        if (student == null)
        {
            return NotFound();
        }
        student.BusId = null;
        await _unitOfWork.Student.UpdateAsync(student);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = busId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToBus(BusDetailsViewModel model)
    {
        var students = await _unitOfWork.Student.FindAllAsync(s => model.SelectedStudentsIds.Contains(s.StudentId));
        if (students == null || !students.Any())
        {
            return NotFound();
        }
        foreach(var student in students)
        {
            student.BusId = model.BusId;
        }

        await _unitOfWork.Student.UpdateRangeAsync(students);
        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = model.BusId });
    }
}
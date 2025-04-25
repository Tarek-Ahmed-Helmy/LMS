using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.AdminRole)]
public class TransportController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    // this gust to display data is not in database
    private static readonly List<(int BusId, ScheduleDetailsViewModel Schedule)> _placeholderSchedules = new List<(int, ScheduleDetailsViewModel)>
    {
        (1, new ScheduleDetailsViewModel { DepartureTime = DateTime.Today.AddHours(7).AddMinutes(30), ArrivalTime = DateTime.Today.AddHours(8).AddMinutes(15), DriverId = "driver1" }),
        (1, new ScheduleDetailsViewModel { DepartureTime = DateTime.Today.AddHours(15).AddMinutes(0), ArrivalTime = DateTime.Today.AddHours(15).AddMinutes(45), DriverId = "driver2" })
    };

    public TransportController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var buses = await _unitOfWork.Bus.FindAllAsync(includes: new[] { "Students", "Students.Class" });
        var viewModel = buses.Select(b => new BusListViewModel
        {
            BusId = b.BusId,
            DriverName = b.DriverName,
            Capacity = b.Capacity,
            Route = b.Route,
            DriverContact = b.DriverContact,
            StudentCount = b.Students?.Count ?? 0,
            Classes = b.Students?.GroupBy(s => s.Class)
                .Select(g => new ClassSummaryViewModel
                {
                    ClassId = g.Key.ClassId,
                    GradeLevel = g.Key.GradeLevel,
                    ClassNumber = g.Key.ClassNumber,
                    StudentCount = g.Count()
                }).ToList() ?? new List<ClassSummaryViewModel>()
        }).ToList();

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateBusViewModel());
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
    public async Task<IActionResult> Routes()
    {
        var buses = await _unitOfWork.Bus.FindAllAsync(includes: new[] { "Students", "Students.Class" });
        var viewModel = buses
            .GroupBy(b => b.Route)
            .Select(g => new RouteListViewModel
            {
                RouteName = g.Key,
                Buses = g.Select(b => new BusSummaryViewModel
                {
                    BusId = b.BusId,
                    DriverName = b.DriverName,
                    Capacity = b.Capacity,
                    StudentCount = b.Students?.Count ?? 0,
                    Classes = b.Students?.GroupBy(s => s.Class)
                        .Select(c => new ClassSummaryViewModel
                        {
                            ClassId = c.Key.ClassId,
                            GradeLevel = c.Key.GradeLevel,
                            ClassNumber = c.Key.ClassNumber,
                            StudentCount = c.Count()
                        }).ToList() ?? new List<ClassSummaryViewModel>()
                }).ToList()
            }).ToList();

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Schedule(int id, [FromQuery] string? userId, [FromQuery] DateTime? newDeparture, [FromQuery] DateTime? newArrival, [FromQuery] string? newDriverId)
    {
        var bus = await _unitOfWork.Bus.FindAsync(b => b.BusId == id, new[] { "Students" });
        if (bus == null)
        {
            TempData["StatusMessage"] = "Bus not found.";
            return RedirectToAction(nameof(Index));
        }

        if (newDeparture.HasValue && newArrival.HasValue && !string.IsNullOrEmpty(newDriverId))
        {
            _placeholderSchedules.Add((id, new ScheduleDetailsViewModel
            {
                DepartureTime = newDeparture.Value,
                ArrivalTime = newArrival.Value,
                DriverId = newDriverId
            }));
            TempData["StatusMessage"] = "Schedule entry added successfully.";
        }

        var schedules = _placeholderSchedules
            .Where(s => s.BusId == id)
            .Select(s => s.Schedule)
            .ToList();

        var filteredSchedules = string.IsNullOrEmpty(userId)
            ? schedules
            : schedules.Where(s => s.DriverId == userId).ToList();

        var viewModel = new BusScheduleViewModel
        {
            Bus = new BusDetailsViewModel
            {
                BusId = bus.BusId,
                Route = bus.Route,
                DriverName = bus.DriverName,
                Capacity = bus.Capacity,
                StudentCount = bus.Students?.Count ?? 0
            },
            Schedules = filteredSchedules
        };

        return View(viewModel);
    }
}
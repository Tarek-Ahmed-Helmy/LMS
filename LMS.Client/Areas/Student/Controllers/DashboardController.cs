using LMS.Entities.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.StudentArea.Controllers;

[Area("Student")]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public DashboardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IActionResult> Index()
    {
        return View();
    }
}

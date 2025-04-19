using LMS.Entities.Interfaces;
using LMS.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.ParentArea.Controllers;

[Area("Parent")]
[Authorize(Roles = SD.ParentRole)]
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

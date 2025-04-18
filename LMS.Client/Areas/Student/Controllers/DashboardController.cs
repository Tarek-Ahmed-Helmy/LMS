using LMS.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.StudentArea.Controllers;

[Area("Student")]
[Authorize(Roles = SD.StudentRole)]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

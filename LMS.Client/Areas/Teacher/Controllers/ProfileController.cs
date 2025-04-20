using LMS.Entities.Interfaces;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Areas.TeacherArea.Controllers;

[Area("Teacher")]
[Authorize(Roles = SD.TeacherRole)]
public class ProfileController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ProfileController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var teacher = await _unitOfWork.Teacher.FindAsync(s => s.TeacherId == id, ["ApplicationUser"]);

        if (teacher == null || teacher.ApplicationUser == null)
            return NotFound();

        var teacherProfileVM = new TeacherProfileViewModel
        {
            FullName = teacher.ApplicationUser.FullName,
            Address = teacher.ApplicationUser.Address,
            ProfilePictureURL = teacher.ApplicationUser.ProfilePictureURL,
            HireDate = teacher.HireDate,
            Qualification = teacher.Qualification,
            Experience = teacher.Experience
        };

        return View(teacherProfileVM);
    }

    [HttpGet]
    public async Task<IActionResult> ReceivedNotifications()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var teacher = await _unitOfWork.Teacher.FindAsync(s => s.TeacherId == id, ["ApplicationUser.ReceivedNotifications.Sender"]);

        if (teacher == null || teacher.ApplicationUser == null) return NotFound();

        var notifications = teacher.ApplicationUser.ReceivedNotifications?
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationViewModel
            {
                NotificationId = n.NotificationId,
                Message = n.Message,
                NotificationType = n.NotificationType,
                CreatedAt = n.CreatedAt,
                SenderName = n.Sender?.FullName ?? "System",
                IsRead = n.IsRead
            }).ToList();

        return View(notifications);
    }

    [HttpPost]
    public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
    {
        var notification = await _unitOfWork.Notification.GetByIdAsync(notificationId);
        if (notification == null || notification.IsRead)
            return Json(new { success = false });

        notification.IsRead = true;
        notification.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
        return Json(new { success = true });
    }
}

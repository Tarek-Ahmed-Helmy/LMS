using LMS.Entities.Interfaces;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Areas.ParentArea.Controllers;

[Area("Parent")]
[Authorize(Roles = SD.ParentRole)]
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

        var parent = await _unitOfWork.Parent.FindAsync(s => s.ParentId == id, ["ApplicationUser", ]);

        if (parent == null || parent.ApplicationUser == null)
            return NotFound();

        var parentProfileVM = new ParentDetailsViewModel
        {
            ParentId = parent.ParentId,
            FullName = parent.ApplicationUser?.FullName,
            Address = parent.ApplicationUser?.Address,
            ProfilePictureURL = parent.ApplicationUser?.ProfilePictureURL,
            Email = parent.ApplicationUser?.Email,
            PhoneNumber = parent.ApplicationUser?.PhoneNumber,
            Occupation = parent.Occupation
        };
        return View(parentProfileVM);
    }

    [HttpGet]
    public async Task<IActionResult> ReceivedNotifications()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var parent = await _unitOfWork.Parent.FindAsync(s => s.ParentId == id, ["ApplicationUser.ReceivedNotifications.Sender"]);

        if (parent == null || parent.ApplicationUser == null) return NotFound();

        var notifications = parent.ApplicationUser.ReceivedNotifications?
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

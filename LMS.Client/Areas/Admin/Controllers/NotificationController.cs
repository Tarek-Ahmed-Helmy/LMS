using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Areas.AdminArea.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.AdminRole)]
public class NotificationController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public NotificationController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<JsonResult> GetAllUsersData()
    {
        var users = await _unitOfWork.ApplicationUser.GetAllAsync();
        return Json(new { data = users });
    }

    [HttpGet]
    public async Task<JsonResult> GetAllNotifications()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notifications = await _unitOfWork.Notification.FindAllAsync(n => n.SenderId == id);
        return Json(new { data = notifications });
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Send(string id)
    {
        var receiver = await _unitOfWork.ApplicationUser.GetByIdAsync(id);
        if (receiver == null)
        {
            return NotFound();
        }

        var model = new NotificationViewModel
        {
            ReceiverId = id,
            ReceiverName = receiver.FullName
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(NotificationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var notification = new Notification
            {
                Message = model.Message,
                NotificationType = model.NotificationType,
                SenderId = currentUserId,
                ReceiverId = model.ReceiverId
            };

            await _unitOfWork.Notification.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        var receiver = await _unitOfWork.ApplicationUser.GetByIdAsync(model.ReceiverId);
        model.ReceiverName = receiver?.FullName;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var notification = await _unitOfWork.Notification.FindAsync(n => n.NotificationId == id, ["Sender", "Receiver"]);

        if (notification == null)
        {
            return NotFound();
        }

        var model = new NotificationViewModel
        {
            NotificationId = notification.NotificationId,
            Message = notification.Message,
            NotificationType = notification.NotificationType,
            SenderName = notification.Sender?.FullName,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var notification = await _unitOfWork.Notification.FindAsync(n => n.NotificationId == id);
        if (notification == null)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (notification.SenderId != currentUserId)
        {
            return Forbid();
        }

        var model = new NotificationViewModel
        {
            NotificationId = notification.NotificationId,
            Message = notification.Message,
            NotificationType = notification.NotificationType,
            ReceiverId = notification.ReceiverId
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NotificationViewModel model)
    {
        if (id != model.NotificationId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var notification = await _unitOfWork.Notification.FindAsync(n => n.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (notification.SenderId != currentUserId)
            {
                return Forbid();
            }

            notification.Message = model.Message;
            notification.NotificationType = model.NotificationType;
            notification.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Notification.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var notification = await _unitOfWork.Notification.FindAsync(n => n.NotificationId == id);
        if (notification == null)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (notification.SenderId != currentUserId)
        {
            return Forbid();
        }

        await _unitOfWork.Notification.DeleteAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}

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
public class ProfileController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProfileController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;

    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var teacher = await _unitOfWork.Teacher.FindAsync(s => s.TeacherId == id, ["ApplicationUser", "Schedules.Class", "Schedules.Subject"]);

        if (teacher == null || teacher.ApplicationUser == null)
            return NotFound();

        var teacherProfileVM = new TeacherDetailsViewModel
        {
            FullName = teacher.ApplicationUser.FullName,
            Address = teacher.ApplicationUser.Address,
            ProfilePictureURL = teacher.ApplicationUser.ProfilePictureURL,
            HireDate = teacher.HireDate,
            Qualification = teacher.Qualification,
            Experience = teacher.Experience,
            Schedules = teacher.Schedules.ToList() ?? new List<Schedule>()
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


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadPhoto(IFormFile File)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var teacher = await _unitOfWork.Teacher.FindAsync(t => t.TeacherId == id, ["ApplicationUser"]);
        if (teacher == null) return NotFound();

        // Handle file upload only if the file is not null and has content
        if (File != null && File.Length > 0)
        {
            // Delete old image if exists
            if (!string.IsNullOrEmpty(teacher.ApplicationUser?.ProfilePictureURL))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                    teacher.ApplicationUser.ProfilePictureURL.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Save new image
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profile-images");
            var fileName = Guid.NewGuid() + Path.GetExtension(File.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            var uploadDirectory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await File.CopyToAsync(stream);
            }

            teacher.ApplicationUser.ProfilePictureURL = $"/uploads/profile-images/{fileName}";
        }
        await _unitOfWork.ApplicationUser.UpdateAsync(teacher.ApplicationUser);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction("Index");
    }  
}


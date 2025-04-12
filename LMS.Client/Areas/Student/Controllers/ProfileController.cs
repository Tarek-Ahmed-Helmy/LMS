using LMS.Entities.Interfaces;
using LMS.Web.ViewModels.StudentViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.StudentArea.Controllers;

[Area("Student")]
public class ProfileController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ProfileController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, ["ApplicationUser", "Class", "Bus"]);

        if (student == null || student.ApplicationUser == null)
            return NotFound();

        var studentProfileVM = new StudentProfileViewModel
        {
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            EmergencyContact = student.EmergencyContact,
            AdmissionDate = student.AdmissionDate,
            StudentNumber = student.StudentNumber,
            FullName = student.ApplicationUser.FullName,
            Address = student.ApplicationUser.Address,
            ProfilePictureURL = student.ApplicationUser.ProfilePictureURL,
            BusNumber = student.BusId?.ToString() ?? "N/A",
            ClassNumber = student.Class?.ClassNumber ?? "N/A",
            GradeLevel = student.Class?.GradeLevel
        };

        return View(studentProfileVM);
    }


    [HttpGet]
    public async Task<IActionResult> ReceivedNotifications(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, ["ApplicationUser.ReceivedNotifications.Sender"]);

        if (student == null || student.ApplicationUser == null) return NotFound();

        var notifications = student.ApplicationUser.ReceivedNotifications?
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

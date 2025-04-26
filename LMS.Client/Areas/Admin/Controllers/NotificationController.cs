using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;

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
        var notifications = await _unitOfWork.Notification.FindAllAsync(n => n.SenderId == id, ["Receiver"]);

        var result = notifications.Select(n => new {
            notificationId = n.NotificationId,
            notificationType = n.NotificationType,
            message = n.Message,
            receiver = n.Receiver?.FullName ?? "Unknown"
        }).ToList();
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        };

        return Json(new { data = result }, options);
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
            SenderName = notification.Sender?.FullName ?? "UnKnown",
            ReceiverName = notification.Receiver?.FullName ?? "UnKnown",
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

    [HttpGet]
    public async Task<IActionResult> SendToAll()
    {
        await PopulateViewBags();
        return View();
    }

    #region my code
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> SendToAll(NotificationToAllViewModel model)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        await PopulateViewBags();
    //        return View(model);
    //    }

    //    try
    //    {
    //        //var recipients = new List<Student>();
    //        //var notificationTasks = new List<Task>();

    //        // Get students if selected
    //        if (model.SendToStudents)
    //        {
    //            var recipients = new List<Student>();

    //            if (model.SelectedClassIds.Any())
    //            {
    //                var studentsInClass = (await _unitOfWork.Class.FindAllAsync(c => model.SelectedClassIds.Contains(c.ClassId), ["Students"])).SelectMany(c => c.Students);
    //                recipients.AddRange(studentsInClass);
    //            }

    //            if (model.SelectedSubjectIds.Any())
    //            {
    //                var studentsInSubject = (await _unitOfWork.Subject.FindAllAsync(s => model.SelectedSubjectIds.Contains(s.SubjectId), ["Schedules.Class.Students"]))
    //                    .SelectMany(c => c.Schedules)
    //                    .Select(sh => sh.Class).SelectMany(c => c.Students);
    //                recipients.AddRange(studentsInSubject);
    //            }

    //            if (model.SelectedBusIds.Any())
    //            {
    //                var studentsInBus = (await _unitOfWork.Bus.FindAllAsync(b => model.SelectedBusIds.Contains(b.BusId), ["Students"]))
    //                    .SelectMany(b => b.Students);
    //                recipients.AddRange(studentsInBus);
    //            }

    //            // Remove duplicates
    //            recipients = recipients.Distinct().ToList();

    //            var notifications = recipients.Select(recipient => new Notification
    //            {
    //                NotificationType = model.NotificationType,
    //                Message = model.Message,
    //                ReceiverId = recipient.StudentId,
    //                SenderId = User.FindFirstValue(ClaimTypes.NameIdentifier)
    //            }).ToList();

    //            // Add all notifications in one operation
    //            await _unitOfWork.Notification.AddRangeAsync(notifications);
    //            await _unitOfWork.SaveChangesAsync();
    //        }

    //        // Get parents if selected
    //        if (model.SendToParents)
    //        {
    //            var recipients = new List<Student>();

    //            if (model.SelectedClassIds.Any())
    //            {
    //                var parentsInClass = (await _unitOfWork.Class.FindAllAsync(c => model.SelectedClassIds.Contains(c.ClassId), ["Students"]))
    //                    .SelectMany(c => c.Students);
    //                recipients.AddRange(parentsInClass);
    //            }

    //            if (model.SelectedSubjectIds.Any())
    //            {
    //                var parentsInSubject = (await _unitOfWork.Subject.FindAllAsync(s => model.SelectedSubjectIds.Contains(s.SubjectId), ["Schedules.Class.Students"]))
    //                    .SelectMany(c => c.Schedules)
    //                    .Select(sh => sh.Class).SelectMany(c => c.Students);
    //                recipients.AddRange(parentsInSubject);
    //            }

    //            if (model.SelectedBusIds.Any())
    //            {
    //                var parentsInBus = (await _unitOfWork.Bus.FindAllAsync(b => model.SelectedBusIds.Contains(b.BusId), ["Students"]))
    //                    .SelectMany(b => b.Students);
    //                recipients.AddRange(parentsInBus);
    //            }

    //            // Remove duplicates
    //            recipients = recipients.Distinct().ToList();

    //            var notifications = recipients.Select(recipient => new Notification
    //            {
    //                NotificationType = model.NotificationType,
    //                Message = model.Message,
    //                ReceiverId = recipient.ParentId,
    //                SenderId = User.FindFirstValue(ClaimTypes.NameIdentifier)
    //            }).ToList();

    //            // Add all notifications in one operation
    //            await _unitOfWork.Notification.AddRangeAsync(notifications);
    //            await _unitOfWork.SaveChangesAsync();
    //        }

    //        // Get teachers if selected
    //        if (model.SendToTeachers)
    //        {
    //            var recipients = new List<Teacher>();

    //            if (model.SelectedClassIds.Any())
    //            {
    //                var teachersInClass = (await _unitOfWork.Class.FindAllAsync(c => model.SelectedClassIds.Contains(c.ClassId), ["Schedules.Teacher"]))
    //                    .SelectMany(c => c.Schedules)
    //                    .Select(sh => sh.Teacher);
    //                recipients.AddRange(teachersInClass);
    //            }

    //            if (model.SelectedSubjectIds.Any())
    //            {
    //                var teachersInSubject = (await _unitOfWork.Subject.FindAllAsync(s => model.SelectedSubjectIds.Contains(s.SubjectId), ["Schedules.Teacher"]))
    //                    .SelectMany(c => c.Schedules)
    //                    .Select(sh => sh.Teacher);
    //                recipients.AddRange(teachersInSubject);
    //            }

    //            // Remove duplicates
    //            recipients = recipients.Distinct().ToList();

    //            var notifications = recipients.Select(recipient => new Notification
    //            {
    //                NotificationType = model.NotificationType,
    //                Message = model.Message,
    //                ReceiverId = recipient.TeacherId,
    //                SenderId = User.FindFirstValue(ClaimTypes.NameIdentifier)
    //            }).ToList();

    //            // Add all notifications in one operation
    //            await _unitOfWork.Notification.AddRangeAsync(notifications);
    //            await _unitOfWork.SaveChangesAsync();
    //        }

    //        return RedirectToAction(nameof(SendToAll));
    //    }
    //    catch (Exception ex)
    //    {
    //        ModelState.AddModelError("", "An error occurred while sending notifications. Please try again.");

    //        await PopulateViewBags();
    //        return View(model);
    //    }
    //}
    #endregion

    private async Task PopulateViewBags()
    {
        var classes = await _unitOfWork.Class.GetAllAsync();
        ViewBag.Classes = classes
            .OrderBy(c => c.GradeLevel)
            .ThenBy(c => c.ClassNumber)
            .Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = $"{c.GradeLevel} - {c.ClassNumber}"
            }).ToList();

        var subjects = await _unitOfWork.Subject.GetAllAsync();
        ViewBag.Subjects = subjects
            .OrderBy(s => s.SubjectCode)
            .ThenBy(s => s.SubjectName)
            .Select(s => new SelectListItem
            {
                Value = s.SubjectId.ToString(),
                Text = $"{s.SubjectCode} - {s.SubjectName}"
            }).ToList();

        var buses = await _unitOfWork.Bus.GetAllAsync();
        ViewBag.Buses = buses
            .OrderBy(b => b.BusId)
            .Select(b => new SelectListItem
            {
                Value = b.BusId.ToString(),
                Text = $"{b.BusId} - {b.Route}"
            }).ToList();

        // Get counts using your repository
        ViewBag.TotalStudents = await _unitOfWork.Student.CountAsync();
        ViewBag.TotalParents = await _unitOfWork.Parent.CountAsync();
        ViewBag.TotalTeachers = await _unitOfWork.Teacher.CountAsync();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendToAll(NotificationToAllViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateViewBags();
            return View(model);
        }

        try
        {
            var recipients = new List<ApplicationUser?>();

            if (!model.SendToStudents && !model.SendToParents && !model.SendToTeachers)
            {
                ModelState.AddModelError("", "You must select at least one recipient type (Students, Parents, or Teachers).");
                await PopulateViewBags();
                return View(model);
            }

            if (!model.SelectedClassIds.Any() && !model.SelectedSubjectIds.Any() && !model.SelectedBusIds.Any())
            {
                ModelState.AddModelError("", "You must select at least one filter (Class, Subject, or Bus).");
                await PopulateViewBags();
                return View(model);
            }

            if (model.SendToStudents)
            {
                var studentIncludes = new[] { "ApplicationUser", "Class.Schedules"};
                Expression<Func<Student, bool>> studentCriteria = s =>
                    model.SelectedClassIds.Contains(s.ClassId) ||
                    (s.BusId.HasValue && model.SelectedBusIds.Contains(s.BusId.Value)) ||
                    (s.Class != null && s.Class.Schedules != null && s.Class.Schedules.Any(sc => model.SelectedSubjectIds.Contains(sc.SubjectId)));

                var students = await _unitOfWork.Student.FindAllAsync(studentCriteria, studentIncludes);
                recipients.AddRange(students.Where(s => s.ApplicationUser != null).Select(s => s.ApplicationUser));
            }

            if (model.SendToParents)
            {
                var parentIncludes = new[] { "ApplicationUser", "Childrens", "Childrens.Class" };
                Expression<Func<Parent, bool>> parentCriteria = p =>
                    p.Childrens != null && p.Childrens.Any(s =>
                        model.SelectedClassIds.Contains(s.ClassId) ||
                        (s.BusId.HasValue && model.SelectedBusIds.Contains(s.BusId.Value)) ||
                        (s.Class != null && s.Class.Schedules != null && s.Class.Schedules.Any(sc => model.SelectedSubjectIds.Contains(sc.SubjectId)))
                    );

                var parents = await _unitOfWork.Parent.FindAllAsync(parentCriteria, parentIncludes);
                recipients.AddRange(parents.Where(p => p.ApplicationUser != null).Select(p => p.ApplicationUser));
            }

            if (model.SendToTeachers)
            {
                var teacherIncludes = new[] { "ApplicationUser", "Schedules" };
                Expression<Func<Teacher, bool>> teacherCriteria = t =>
                    t.Schedules != null && t.Schedules.Any(s =>
                        model.SelectedClassIds.Contains(s.ClassId) ||
                        model.SelectedSubjectIds.Contains(s.SubjectId));

                var teachers = await _unitOfWork.Teacher.FindAllAsync(teacherCriteria, teacherIncludes);
                recipients.AddRange(teachers.Where(t => t.ApplicationUser != null).Select(t => t.ApplicationUser));
            }

            // Remove duplicates
            recipients = recipients.GroupBy(u => u?.Id)
                              .Select(g => g.First())
                              .ToList();

            if (!recipients.Any())
            {
                ModelState.AddModelError("", "No recipients match the selected filters.");
                await PopulateViewBags();
                return View(model);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Create notifications
            var notifications = recipients.Select(recipient => new Notification
            {
                NotificationType = model.NotificationType,
                Message = model.Message,
                ReceiverId = recipient.Id,
                IsRead = false,
                SenderId = currentUserId
            }).ToList();

            await _unitOfWork.Notification.AddRangeAsync(notifications);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "An error occurred while sending notifications. Please try again.");
            await PopulateViewBags();
            return View(model);
        }
    }
}

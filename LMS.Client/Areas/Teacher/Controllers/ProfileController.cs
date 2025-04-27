using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NuGet.DependencyResolver;
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

        ViewBag.id = id;

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

    [HttpGet]

    public async Task<IActionResult> Update(string id)
    {
        var teacher = await _unitOfWork.Teacher.FindAsync(t => t.TeacherId == id, includes: new string[] { "ApplicationUser" });
        if (teacher == null) return NotFound();
        

        var teacherProfileVM = new ProfileTeacherUpdateViewModel
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

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Update( string id , ProfileTeacherUpdateViewModel updateVM)
    //{

    //        var teacher = await _unitOfWork.Teacher.FindAsync(s => s.TeacherId == id, ["ApplicationUser"]);
    //        if (teacher == null )
    //            return NotFound();


    //    if (ModelState.IsValid)
    //    {            // Update in ApplicationUser
    //        teacher.UpdatedAt = DateTime.UtcNow;
    //        teacher.ApplicationUser.FullName = updateVM.FullName;
    //        teacher.ApplicationUser.Address = updateVM.Address;
    //        // update in Teacher
    //        teacher.HireDate = updateVM.HireDate;
    //        teacher.Qualification = updateVM.Qualification;
    //        teacher.Experience = updateVM.Experience;

    //        if (updateVM.ProfilePictureURL != null)
    //        {
    //            teacher.ApplicationUser.ProfilePictureURL = updateVM.ProfilePictureURL;
    //        }
    //    }
    //    await _unitOfWork.ApplicationUser.UpdateAsync(teacher.ApplicationUser);
    //    await _unitOfWork.Teacher.UpdateAsync(teacher);
    //    // Save changes to the databas
    //    await _unitOfWork.SaveChangesAsync();
    //    TempData["success"] = "Profile updated successfully!";
    //    return RedirectToAction("Index");
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string id, ProfileTeacherUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var teacher = await _unitOfWork.Teacher.FindAsync(t => t.TeacherId == id, includes: new string[] { "ApplicationUser" });
        if (teacher == null) return NotFound();


            //var applicationUser =  await _unitOfWork.ApplicationUser.FindAsync(u => u.Id = id)

            if (teacher == null)
            {
                TempData["error"] = "Teacher not found";
                return RedirectToAction("Index");
            }

            // Update properties
            teacher.ApplicationUser.FullName = model.FullName;
            teacher.ApplicationUser.Address = model.Address;
            teacher.HireDate = model.HireDate;
            teacher.Qualification = model.Qualification;
            teacher.Experience = model.Experience;
            teacher.ApplicationUser.UpdatedAt = DateTime.UtcNow;

            // Handle profile picture upload
            if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(teacher.ApplicationUser.ProfilePictureURL))
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
                var uniqueFileName = $"{Guid.NewGuid()}_{model.ProfilePictureFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                Directory.CreateDirectory(uploadsFolder);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePictureFile.CopyToAsync(fileStream);
                }

                teacher.ApplicationUser.ProfilePictureURL = $"/uploads/profile-images/{uniqueFileName}";
            }

            await _unitOfWork.ApplicationUser.UpdateAsync(teacher.ApplicationUser);
            await _unitOfWork.Teacher.UpdateAsync(teacher);
            await _unitOfWork.SaveChangesAsync();

            TempData["success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }

        
    }


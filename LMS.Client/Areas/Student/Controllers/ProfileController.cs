using LMS.Entities.Interfaces;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Areas.StudentArea.Controllers;

[Area("Student")]
[Authorize(Roles = SD.StudentRole)]
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
    public async Task<IActionResult> ReceivedNotifications()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

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

    [HttpGet]
    public async Task<IActionResult> ExamResults()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, ["ApplicationUser", "ExamResults.Exam", "ExamResults.Exam.Subject"]);

        if (student == null)
            return NotFound();

        var viewModel = new StudentExamResultsViewModel
        {
            StudentId = student.StudentId,
            FullName = student.ApplicationUser?.FullName ?? "N/A",
            Results = student.ExamResults?.Select(er => new ExamResultViewModel
            {
                Subject = er.Exam?.Subject?.SubjectName ?? "N/A",
                ExamType = er.Exam.ExamType,
                Score = er.Score,
                TotalMarks = er.Exam.TotalMarks,
                ExamDate = er.Exam?.ExamDate ?? DateTime.MinValue,
                Remarks = er.Remarks ?? ""
            }).ToList() ?? new List<ExamResultViewModel>()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Payments()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, ["ApplicationUser", "Payments"]);

        if (student == null)
            return NotFound();

        var vm = new StudentPaymentsViewModel
        {
            FullName = student.ApplicationUser?.FullName ?? "N/A",
            StudentId = student.StudentId,
            Payments = student.Payments?.Select(p => new PaymentRecordViewModel
            {
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                Method = p.PaymentMethod.ToString(),
                Status = p.PaymentStatus.ToString(),
                ReceiptPath = p.ReceiptPath
            }).OrderByDescending(p => p.PaymentDate).ToList() ?? new()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> ClassSchedule()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, ["ApplicationUser", "Class"]);
        

        if (student == null || student.ClassId == null)
            return NotFound();

        var schedule = await _unitOfWork.Schedule.FindAllAsync(s => s.ClassId == student.ClassId, ["Subject", "Teacher.ApplicationUser"]);

        var grouped = schedule
            .GroupBy(s => s.DayOfWeek.ToString())
            .OrderBy(g => Enum.Parse<DayOfWeek>(g.Key))
            .ToDictionary(
                g => g.Key,
                g => g.Select(s => new ScheduleItemViewModel
                {
                    Day = s.DayOfWeek.ToString(),
                    StartTime = s.StartTime.ToString(@"hh\:mm"),
                    EndTime = s.EndTime.ToString(@"hh\:mm"),
                    Subject = s.Subject?.SubjectName ?? "Unknown",
                    Teacher = s.Teacher?.ApplicationUser?.FullName ?? "Unknown"
                }).OrderBy(x => x.StartTime).ToList()
            );

        var vm = new StudentScheduleViewModel
        {
            FullName = student.ApplicationUser?.FullName ?? "N/A",
            StudentId = student.StudentId,
            GroupedSchedule = grouped
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> ExamSchedule(DateTime? fromDate, DateTime? toDate)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, ["ApplicationUser", "Class"]);


        if (student == null || student.ClassId == null)
            return NotFound();

        var exams = await _unitOfWork.Exam.FindExamScheduleAsync(e => e.ClassID == student.ClassId, fromDate, toDate, ["Subject", "Teacher.ApplicationUser"]);

        var groupedExams = exams
            .GroupBy(e => e.ExamType.ToString())
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => new ExamScheduleItemViewModel
                {
                    Subject = e.Subject?.SubjectName ?? "N/A",
                    Teacher = e.Teacher?.ApplicationUser?.FullName ?? "N/A",
                    Date = e.ExamDate.ToString("yyyy-MM-dd"),
                    TotalMarks = e.TotalMarks,
                    DurationMinutes = e.ExamDuration
                }).OrderBy(e => e.Date).ToList()
            );

        var vm = new StudentExamScheduleViewModel
        {
            StudentId = student.StudentId,
            FullName = student.ApplicationUser?.FullName ?? "N/A",
            FromDate = fromDate,
            ToDate = toDate,
            GroupedExams = groupedExams
        };

        return View(vm);
    }
}

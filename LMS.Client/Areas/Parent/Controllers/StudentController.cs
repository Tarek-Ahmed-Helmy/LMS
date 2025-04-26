using LMS.Entities.Interfaces;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Areas.ParentArea.Controllers;

[Area("Parent")]
[Authorize(Roles = SD.ParentRole)]
public class StudentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> Assignments(string? studentId = null)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var children = await _unitOfWork.Student.FindAllAsync(
            s => s.ParentId == id,
            ["ApplicationUser", "Class.Schedules.Subject.Assignments.Teacher.ApplicationUser"]
        );

        if (children == null || !children.Any())
            return View(new Dictionary<string, List<AssignmentViewModel>>());

        ViewBag.Children = children.Select(c => new
        {
            Id = c.StudentId,
            Name = c.ApplicationUser?.FullName ?? "N/A"
        }).ToList();

        ViewBag.SelectedStudentId = studentId;

        if (!string.IsNullOrEmpty(studentId))
        {
            children = children.Where(c => c.StudentId == studentId).ToList();
        }

        var groupedAssignments = new Dictionary<string, List<AssignmentViewModel>>();
        var now = DateTime.UtcNow;


        foreach (var child in children)
        {
            var studentName = child.ApplicationUser?.FullName ?? "N/A";

            if (child.Class?.Schedules == null)
                continue;

            var assignments = child.Class.Schedules
                .Where(s => s.Subject?.Assignments != null)
                .SelectMany(s => s.Subject.Assignments)
                .Select(a => new AssignmentViewModel
                {
                    Subject = a.Subject?.SubjectName ?? "N/A",
                    Title = a.Title,
                    Description = a.AssignmentDescription ?? "N/A",
                    DueDate = a.Deadline,
                    Status = a.Deadline < now ? "Overdue" : "Upcoming"
                })
                .OrderBy(a => a.DueDate)
                .ToList();

            if (assignments.Any())
            {
                groupedAssignments[studentName] = assignments;
            }
        }

        return View(groupedAssignments);
    }


    [HttpGet]
    public async Task<IActionResult> Grades()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var submissions = await _unitOfWork.Submission.FindAllAsync(
            s => s.Student.ParentId == id,
            ["Assignment.Subject", "Student.ApplicationUser"]
        );

        var grades = submissions.Select(s => new GradeViewModel
        {
            Student = s.Student?.ApplicationUser?.FullName ?? "N/A",
            Subject = s.Assignment?.Subject?.SubjectName ?? "N/A",
            Score = s.Score >= 0 ? s.Score.ToString() : "--",
            TotalMarks = s.Assignment?.TotalMarks ?? 0,
            Feedback = s.Feedback ?? "N/A"
        }).ToList();

        return View(grades);
    }

    [HttpGet]
    public async Task<IActionResult> ExamResults()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var examResults = await _unitOfWork.ExamResult.FindAllAsync(
            er => er.Student.ParentId == id,
            ["Exam.Subject", "Student.ApplicationUser"]
        );

        var resultVMs = examResults.Select(er => new ExamResultViewModel
        {
            StudentName = er.Student.ApplicationUser?.FullName ?? "N/A",
            Subject = er.Exam.Subject?.SubjectName ?? "N/A",
            ExamType = er.Exam.ExamType,
            ExamDate = er.Exam.ExamDate,
            Score = er.Score,
            TotalMarks = er.Exam.TotalMarks
        }).ToList();

        return View(resultVMs);
    }

    [HttpGet]
    public async Task<IActionResult> Schedule()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var schedules = await _unitOfWork.Schedule.FindAllAsync(
            s => s.Class.Students.Select(s => s.ParentId).Contains(id),
            ["Subject", "Teacher.ApplicationUser", "Class"]
        );

        var scheduleVMs = schedules.Select(s => new ScheduleViewModel
        {
            ClassName = s.Class?.ClassNumber ?? "N/A",
            Subject = s.Subject?.SubjectName ?? "N/A",
            Teacher = s.Teacher?.ApplicationUser?.FullName ?? "N/A",
            DayOfWeek = s.DayOfWeek.ToString(),
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }).ToList();

        return View(scheduleVMs);
    }
}

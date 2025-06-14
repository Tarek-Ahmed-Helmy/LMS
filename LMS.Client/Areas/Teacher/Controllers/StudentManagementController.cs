﻿using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Areas.TeacherArea.Controllers;

[Area("Teacher")]
[Authorize(Roles = SD.TeacherRole)]
public class StudentManagementController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentManagementController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Fetch the teacher's schedules to get the classes they teach
        var schedules = await _unitOfWork.Schedule.FindAllAsync( s => s.TeacherId == userId, ["Class.Students.ApplicationUser"]);

        // Get all students from the classes the teacher teaches
        var students = schedules
            .SelectMany(s => s.Class?.Students ?? new List<Student>())
            .Select(s => new TeacherStudentListViewModel
            {
                StudentId = s.StudentId,
                FullName = s.ApplicationUser?.FullName ?? "N/A",
                StudentNumber = s.StudentNumber,
                GradeLevel = s.Class.GradeLevel,
                ClassNumber = s.Class?.ClassNumber ?? "Not Assigned"
            })
            .OrderBy(s => s.FullName)
            .ToList();

        return View(students);
    }

    public async Task<IActionResult> Details(string id)
    {
        var student = await _unitOfWork.Student.FindAsync(
            s => s.StudentId == id, ["ApplicationUser", "Class", "Attendances", "ExamResults.Exam.Subject", "Submissions.Assignment"]);

        if (student == null)
        {
            return NotFound();
        }

        // Calculate performance metrics
        var attendances = student.Attendances?.ToList() ?? new List<Attendance>();
        var totalDays = attendances?.Count;
        var presentDays = attendances.Count(a => a.Status == AttendanceStatus.Present);
        var attendancePercentage = totalDays > 0 ? (presentDays * 100.0 / totalDays) : 0;

        var examResults = student.ExamResults?.ToList() ?? new List<ExamResult>();
        var averageExamScore = examResults.Any() ? examResults.Average(er => er.Score) : 0;

        var submissions = student.Submissions?.ToList() ?? new List<Submission>();
        var averageAssignmentScore = submissions.Any() ? submissions.Average(s => s.Score) : 0;

        // Populate the view model
        var viewModel = new TeacherStudentProfileViewModel
        {
            StudentId = student.StudentId,
            FullName = student.ApplicationUser?.FullName ?? "N/A",
            StudentNumber = student.StudentNumber,
            ClassName = student.Class?.ClassNumber ?? "Not Assigned",
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender.ToString(),
            EmergencyContact = student.EmergencyContact,
            AdmissionDate = student.AdmissionDate,
            ProfilePictureURL = student.ApplicationUser?.ProfilePictureURL,
            AverageExamScore = averageExamScore,
            AverageAssignmentScore = averageAssignmentScore,
            RecentAttendances = attendances
                .OrderByDescending(a => a.Date)
                .Take(5)
                .Select(a => new AttendanceViewModel
                {
                    Date = a.Date,
                    Status = a.Status.ToString(),
                    Notes = a.Notes
                })
                .ToList(),
            ExamResults = examResults
                .Select(er => new ExamResultViewModell
                {
                    ExamId = er.ExamId,
                    ExamType = er.Exam?.ExamType.ToString() ?? "N/A",
                    SubjectName = er.Exam?.Subject?.SubjectName ?? "N/A",
                    Score = er.Score,
                    TotalMarks = er.Exam?.TotalMarks ?? 0,
                    Remarks = er.Remarks
                })
                .ToList(),
            Submissions = submissions
                .Select(s => new SubmissionViewModell
                {
                    AssignmentId = s.AssignmentId,
                    AssignmentTitle = s.Assignment?.Title ?? "N/A",
                    SubmissionDate = s.SubmissionDate,
                    Score = s.Score,
                    TotalMarks = s.Assignment?.TotalMarks ?? 0,
                    Feedback = s.Feedback
                })
                .ToList()
        };

        return View(viewModel);
    }

}
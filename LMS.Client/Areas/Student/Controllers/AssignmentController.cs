using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels.StudentViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Areas.StudentArea.Controllers;

[Area("Student")]
[Authorize(Roles = SD.StudentRole)]
public class AssignmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public AssignmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await _unitOfWork.Student.FindAsync(
            s => s.StudentId == id,
            includes: new[]
            {
            "Class.Schedules.Subject.Assignments.Teacher.ApplicationUser",
            "Class.Schedules.Subject.Assignments.Submissions"
            }
        );

        if (student?.Class?.Schedules == null || !student.Class.Schedules.Any())
            return View(new List<StudentAssignmentViewModel>()); // No class or schedules

        var now = DateTime.UtcNow;
        var assignments = student.Class.Schedules
            .Where(s => s.Subject?.Assignments != null)
            .SelectMany(s => s.Subject.Assignments)
            .Where(a => a.Deadline > now)
            .ToList();

        if (!assignments.Any())
            return View(new List<StudentAssignmentViewModel>()); // No active assignments

        var viewModel = assignments.Select(a =>
        {
            var submission = a.Submissions?.FirstOrDefault(s => s.StudentId == id);
            string progress = GetSubmissionProgress(submission, a.TotalMarks, out string grade);

            return new StudentAssignmentViewModel
            {
                AssignmentId = a.AssignmentId,
                Title = a.Title,
                Progress = progress,
                Grade = grade,
                Subject = a.Subject?.SubjectName ?? "N/A",
                Teacher = a.Teacher?.ApplicationUser?.FullName ?? "N/A",
                ExpiresAt = a.Deadline
            };
        }).ToList();

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int assignmentId)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var assignment = await _unitOfWork.Assignment.FindAsync(
            a => a.AssignmentId == assignmentId,
            includes: new[] { "Subject", "Teacher.ApplicationUser", "Submissions" }
        );

        if (assignment == null)
            return NotFound();

        var submission = assignment.Submissions?.FirstOrDefault(s => s.StudentId == id);
        string progress = GetSubmissionProgress(submission, assignment.TotalMarks, out string grade);

        var viewModel = new AssignmentDetailsViewModel
        {
            AssignmentId = assignment.AssignmentId,
            Title = assignment.Title,
            Description = assignment.AssignmentDescription,
            Subject = assignment.Subject?.SubjectName ?? "N/A",
            Teacher = assignment.Teacher?.ApplicationUser?.FullName ?? "N/A",
            ExpiresAt = assignment.Deadline,
            Progress = progress,
            Grade = grade,
            FilePath = submission?.FilePath
        };

        return View(viewModel);
    }
    private static string GetSubmissionProgress(Submission? submission, int totalMarks, out string grade)
    {
        string progress;
        grade = "--";

        if (submission == null)
        {
            progress = "Not Submitted";
        }
        else if (submission.Score >= 0)
        {
            progress = "Graded";
            grade = $"{submission.Score} / {totalMarks}";
        }
        else
        {
            progress = "Pending Grading";
        }

        return progress;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int assignmentId, IFormFile File)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // Validate model state first
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Details), new {assignmentId});

        // Retrieve the assignment using the provided assignmentId
        var assignment = await _unitOfWork.Assignment.FindAsync(a => a.AssignmentId == assignmentId);
        if (assignment == null)
            return NotFound();

        // Retrieve the student using the provided assignmentId
        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id);
        if (student == null)
            return NotFound();

        // Check if the student has already submitted this assignment
        var existingSubmission = await _unitOfWork.Submission.FindAsync(s => s.AssignmentId == assignmentId && s.StudentId == id);
        if (existingSubmission != null)
        {
            ModelState.AddModelError("", "You have already submitted this assignment.");
            return RedirectToAction(nameof(Details), new { assignmentId });
        }

        // Handle file upload only if the file is not null and has content
        string path;
        if (File != null && File.Length > 0)
        {
            // Generate a unique file name to avoid overwriting
            var fileName = Guid.NewGuid() + Path.GetExtension(File.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

            // Ensure the uploads folder exists
            var uploadDirectory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            // Save the file asynchronously
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await File.CopyToAsync(stream);
            }

            // Set the file path in the submission model
            path = "/uploads/" + fileName;
        }
        else
        {
            // If file is required, add an error
            ModelState.AddModelError("File", "Please select a file to upload.");
            return RedirectToAction(nameof(Details), new { assignmentId });
        }

        var submission = new Submission
        {
            AssignmentId = assignmentId,
            StudentId = id,
            Score = -1, // Default score for ungraded submissions
            Feedback = null, // Default feedback for ungraded submissions
            FilePath = path, // Set the file path
            SubmissionDate = DateTime.UtcNow
        };

        // Save the submission to the database
        await _unitOfWork.Submission.AddAsync(submission);
        await _unitOfWork.SaveChangesAsync();

        // Redirect to the student's assignments index page
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Feedback(int assignmentId)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var submission = await _unitOfWork.Submission.FindAsync(
            s => s.AssignmentId == assignmentId && s.StudentId == id,
            includes: new[] { "Assignment", "Student.ApplicationUser" }
        );

        if (submission == null)
            return NotFound();

        var viewModel = new SubmissionFeedbackViewModel
        {
            AssignmentTitle = submission.Assignment.Title,
            StudentFullName = submission.Student.ApplicationUser.FullName,
            SubmissionDate = submission.SubmissionDate,
            Score = submission.Score,
            Feedback = submission.Feedback,
            FilePath = submission.FilePath
        };

        return View(viewModel);
    }
}

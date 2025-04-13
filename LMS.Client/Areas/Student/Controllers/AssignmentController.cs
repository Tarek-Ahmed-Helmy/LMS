using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Web.ViewModels.StudentViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LMS.Web.Areas.StudentArea.Controllers;

[Area("Student")]
public class AssignmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public AssignmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IActionResult> Index(string studentId)
    {
        // Step 1: Get the student along with the Class
        var student = await _unitOfWork.Student.FindAsync(
            s => s.StudentId == studentId,
            includes: new[] { "Class" }
        );

        if (student == null || student.Class == null)
            return NotFound("Student or class not found.");

        var classId = student.Class.ClassId;

        // Step 2: Get the schedules for the student's class (to find the related subjects)
        var schedules = await _unitOfWork.Schedule.FindAllAsync(
            s => s.ClassId == classId,
            includes: new[] { "Subject" }
        );

        var subjectIds = schedules
            .Where(s => s.Subject != null)
            .Select(s => s.Subject.SubjectId)
            .Distinct()
            .ToList();

        // Step 3: Get active assignments related to these subjects
        var assignments = await _unitOfWork.Assignment.FindAllAsync(
            a => subjectIds.Contains(a.SubjectId) && a.Deadline > DateTime.UtcNow,
            includes: new[] { "Subject", "Teacher.ApplicationUser", "Submissions" }
        );

        // Step 4: Map to StudentAssignmentViewModel
        var viewModel = assignments.Select(a =>
        {
            var submission = a.Submissions?.FirstOrDefault(s => s.StudentId == studentId);
            string progress;
            string grade = "--";

            if (submission == null)
            {
                progress = "Not Submitted";
            }
            else if (submission.Score > 0)
            {
                progress = "Graded";
                grade = $"{submission.Score} / {a.TotalMarks}";
            }
            else
            {
                progress = "Submitted - Pending Grading";
            }

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




    public async Task<IActionResult> Details(int id, string studentId)
    {
        var assignment = await _unitOfWork.Assignment.FindAsync(
            a => a.AssignmentId == id,
            includes: new[] { "Subject", "Teacher.ApplicationUser", "Submissions" }
        );

        if (assignment == null)
            return NotFound();

        var submission = assignment.Submissions?.FirstOrDefault(s => s.StudentId == studentId);

        var viewModel = new AssignmentDetailsViewModel
        {
            AssignmentId = assignment.AssignmentId,
            Title = assignment.Title,
            Description = assignment.AssignmentDescription,
            Subject = assignment.Subject?.SubjectName ?? "N/A",
            Teacher = assignment.Teacher?.ApplicationUser?.FullName ?? "N/A",
            ExpiresAt = assignment.Deadline,
            Progress = GetSubmissionProgress(submission),
            Grade = GetGrade(submission, assignment.TotalMarks),
            FilePath = submission?.FilePath
        };

        return View(viewModel);
    }
    private string GetSubmissionProgress(Submission? submission)
    {
        if (submission == null)
            return "Not Submitted";

        return submission.Score > 0 ? "Graded" : "Submitted - Pending Grading";
    }

    private string GetGrade(Submission? submission, int totalMarks)
    {
        if (submission != null && submission.Score > 0)
            return $"{submission.Score} / {totalMarks}";

        return "--";
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int id, string studentId, Submission submission, IFormFile File)
    {
        // Validate model state first
        if (!ModelState.IsValid)
            return View(submission);

        // Retrieve the assignment using the provided id
        var assignment = await _unitOfWork.Assignment.FindAsync(a => a.AssignmentId == id);
        if (assignment == null)
            return NotFound();

        // Retrieve the student using the provided studentId
        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == studentId);
        if (student == null)
            return NotFound();

        // Check if the student has already submitted this assignment
        var existingSubmission = await _unitOfWork.Submission.FindAsync(s => s.AssignmentId == id && s.StudentId == studentId);
        if (existingSubmission != null)
        {
            ModelState.AddModelError("", "You have already submitted this assignment.");
            return View(submission);
        }

        // Handle file upload only if the file is not null and has content
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
            submission.FilePath = "/uploads/" + fileName;
        }
        else
        {
            // If file is required, add an error
            ModelState.AddModelError("File", "Please select a file to upload.");
            return View(submission);
        }

        // Set remaining submission properties
        submission.AssignmentId = id;
        submission.StudentId = studentId;
        submission.SubmissionDate = DateTime.UtcNow;

        // Save the submission to the database
        await _unitOfWork.Submission.AddAsync(submission);
        await _unitOfWork.SaveChangesAsync();

        // Redirect to the student's assignments index page
        return RedirectToAction("Index", new { studentId });
    }



    public async Task<IActionResult> Feedback(int id, string studentId)
    {
        var submission = await _unitOfWork.Submission.FindAsync(
            s => s.AssignmentId == id && s.StudentId == studentId,
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

using System.Security.Claims;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.TeacherArea.Controllers;

[Area("Teacher")]
[Authorize(Roles = SD.TeacherRole)]
public class AssignmentController : Controller
{
    private readonly IUnitOfWork _uow;
    public AssignmentController(IUnitOfWork uow) => _uow = uow;

    /* ===================  Index  =================== */
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var assignments = await _uow.Assignment
            .FindAllAsync(a => a.TeacherId == teacherId, ["Subject"]);

        var vm = assignments
            .GroupBy(a => a.Subject!.SubjectName)
            .Select(g => new AssignmentGroupViewModel
            {
                SubjectName = g.Key,
                Assignments = g.Select(a => new TeacherAssignmentViewModel
                {
                    AssignmentId = a.AssignmentId,
                    Title = a.Title,
                    Description = a.AssignmentDescription,
                    Deadline = a.Deadline,
                    Status = a.Deadline < DateTime.UtcNow ? "Closed" : "Open"
                }).ToList()
            }).ToList();

        return View(vm);
    }

    /* ===================  Upload  =================== */
    [HttpGet]
    public async Task<IActionResult> Upload()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var subjects = await _uow.Subject.FindAllAsync(s => s.Schedules.Select(sh => sh.TeacherId).Contains(userId));
        return View(new CreateAssignmentViewModel { Subjects = subjects });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(CreateAssignmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Subjects = await _uow.Subject.GetAllAsync();
            return View(model);
        }

        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var assignment = new Assignment
        {
            Title = model.Title,
            AssignmentDescription = model.Description,
            Deadline = model.Deadline,
            TotalMarks = model.TotalMarks,
            SubmissionType = model.SubmissionType,
            SubjectId = model.SubjectId,
            TeacherId = teacherId
        };

        await _uow.Assignment.AddAsync(assignment);
        await _uow.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    /* ===================  Edit  =================== */
    [HttpGet]
    public async Task<IActionResult> Edit(int assignmentId)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var assignment = await _uow.Assignment.FindAsync(
            a => a.AssignmentId == assignmentId && a.TeacherId == teacherId);

        if (assignment is null) return NotFound();

        var subjects = await _uow.Subject.GetAllAsync();

        return View(new EditAssignmentViewModel
        {
            AssignmentId = assignment.AssignmentId,
            Title = assignment.Title,
            Description = assignment.AssignmentDescription,
            Deadline = assignment.Deadline,
            TotalMarks = assignment.TotalMarks,
            SubmissionType = assignment.SubmissionType,
            SubjectId = assignment.SubjectId,
            Subjects = subjects
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int assignmentId, EditAssignmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Subjects = await _uow.Subject.GetAllAsync();
            return View(model);
        }

        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var assignment = await _uow.Assignment.FindAsync(
            a => a.AssignmentId == assignmentId && a.TeacherId == teacherId);

        if (assignment is null) return NotFound();

        assignment.Title = model.Title;
        assignment.AssignmentDescription = model.Description;
        assignment.Deadline = model.Deadline;
        assignment.TotalMarks = model.TotalMarks;
        assignment.SubmissionType = model.SubmissionType;
        assignment.SubjectId = model.SubjectId;
        assignment.UpdatedAt = DateTime.UtcNow;

        await _uow.Assignment.UpdateAsync(assignment);
        await _uow.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    /* ===================  Delete  =================== */
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int assignmentId)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var assignment = await _uow.Assignment.FindAsync(
            a => a.AssignmentId == assignmentId && a.TeacherId == teacherId);

        if (assignment is null) return NotFound();

        await _uow.Assignment.DeleteAsync(assignment);
        await _uow.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    /* ===================  Submissions  =================== */
    [HttpGet]
    public async Task<IActionResult> Submissions(int assignmentId)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var assignment = await _uow.Assignment.FindAsync(
            a => a.AssignmentId == assignmentId && a.TeacherId == teacherId);

        if (assignment is null) return NotFound();

        var submissions = await _uow.Submission.FindAllAsync(
            s => s.AssignmentId == assignmentId,
            new[] { "Student.ApplicationUser" });

        ViewBag.AssignmentTitle = assignment.Title;

        var vm = submissions.Select(s => new SubmissionViewModel
        {
            SubmissionId = s.SubmissionId,
            StudentName = s.Student!.ApplicationUser!.FullName,
            SubmissionDate = s.SubmissionDate,
            Score = s.Score,
            FilePath = s.FilePath
        }).ToList();

        return View(vm);
    }

    /* ===================  Grade  =================== */
    [HttpGet]
    public async Task<IActionResult> Grade(int submissionId)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var submission = await _uow.Submission.FindAsync(
            s => s.SubmissionId == submissionId && s.Assignment!.TeacherId == teacherId,
            new[] { "Assignment" });

        if (submission is null)
            return NotFound();

        var vm = new GradeSubmissionViewModel
        {
            SubmissionId = submission.SubmissionId,
            Score = submission.Score ,
            Feedback = submission.Feedback
        };

        ViewBag.AssignmentId = submission.AssignmentId;
        ViewBag.AssignmentTitle = submission.Assignment!.Title;

        return View(vm);
    }

    [HttpPost("Grade/{submissionId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Grade(int submissionId, GradeSubmissionViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var submission = await _uow.Submission.FindAsync(
            s => s.SubmissionId == submissionId && s.Assignment!.TeacherId == teacherId,
            new[] { "Assignment" });

        if (submission is null) return NotFound();

        submission.Score = submission.Score;
        submission.Feedback = submission.Feedback;
        submission.UpdatedAt = DateTime.UtcNow;

        await _uow.Submission.UpdateAsync(submission);
        await _uow.SaveChangesAsync();

        return RedirectToAction(nameof(Submissions), new { assignmentId = submission.AssignmentId });
    }
}

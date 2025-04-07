using LMS.Entities.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.Student.Controllers;

[Area("Student")]
public class AssignmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public AssignmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IActionResult> Index(int studentId)
    {
        var assignments = await _unitOfWork.Assignment.FindAllAsync(a => a.StudentId == studentId);
        return View(assignments);
    }

    public async Task<IActionResult> Details(int id, int studentId)
    {
        var assignment = await _unitOfWork.Assignment.FindAsync(a => a.Id == id && a.StudentId == studentId);
        if (assignment == null) return NotFound();
        return View(assignment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int id, int studentId, AssignmentSubmission submission)
    {
        if (ModelState.IsValid)
        {
            submission.AssignmentId = id;
            submission.StudentId = studentId;
            await _unitOfWork.Assignment.AddAsync(submission);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { studentId });
        }
        return View(submission);
    }

    public async Task<IActionResult> Feedback(int id, int studentId)
    {
        var feedback = await _unitOfWork.Assignment.FindAsync(f => f.AssignmentId == id && f.StudentId == studentId);
        if (feedback == null) return NotFound();
        return View(feedback);
    }

}

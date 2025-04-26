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
[Route("Teacher/[controller]")]
public class ExamController : Controller
{
    private readonly IUnitOfWork _uow;
    public ExamController(IUnitOfWork uow) => _uow = uow;

   
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        string teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var exams = await _uow.Exam
            .FindAllAsync(e => e.TeacherID == teacherId, new[] { "Subject" });

        var vm = exams
            .GroupBy(e => e.Subject!.SubjectName)
            .Select(g => new ExamGroupVM
            {
                SubjectName = g.Key,
                Exams = g.Select(e => new TeacherExamVM
                {
                    ExamId = e.ExamID,
                    Type = e.ExamType.ToString(),
                    ExamDate = e.ExamDate,
                    TotalMarks = e.TotalMarks,
                    Status = e.ExamDate >= DateTime.UtcNow ? "Upcoming" : "Finished"
                }).ToList()
            }).ToList();

        return View(vm);
    }

    
    [HttpGet("uploadExam")]
    public async Task<IActionResult> Upload()
    {
        var vm = new CreateExamVM
        {
            Subjects = await _uow.Subject.GetAllAsync() ?? new List<Subject>(),
            Classes = await _uow.Class.GetAllAsync() ?? new List<Class>()
        };
        return View(vm);
    }

    
    [HttpPost("uploadExam")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(CreateExamVM vm)
    {
        /* Always (re)populate the dropdown sources  */
        vm.Subjects = await _uow.Subject.GetAllAsync() ?? new List<Subject>();
        vm.Classes = await _uow.Class.GetAllAsync() ?? new List<Class>();

        /* If the form is invalid, re-display with messages */
        if (!ModelState.IsValid)
        {
            return View(vm);                 // << returns here
        }

       
        string teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var exam = new Exam
        {
            ExamType = vm.ExamType,
            ExamDate = vm.ExamDate,
            TotalMarks = vm.TotalMarks,
            ExamDuration = vm.ExamDuration,
            SubjectID = vm.SubjectId,
            ClassID = vm.ClassId,
            TeacherID = teacherId
        };

        await _uow.Exam.AddAsync(exam);
        await _uow.SaveChangesAsync();

        return RedirectToAction(nameof(Index));   
    }


   
    [HttpGet("editExam/{examId:int}")]
    public async Task<IActionResult> Edit(int examId)
    {
        string teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var exam = await _uow.Exam.FindAsync(
            e => e.ExamID == examId && e.TeacherID == teacherId);

        if (exam is null) return NotFound();

        var vm = new EditExamVM
        {
            ExamId = exam.ExamID,
            ExamType = exam.ExamType,
            ExamDate = exam.ExamDate,
            TotalMarks = exam.TotalMarks,
            ExamDuration = exam.ExamDuration,
            SubjectId = exam.SubjectID,
            ClassId = exam.ClassID,
            Subjects = await _uow.Subject.GetAllAsync(),
            Classes = await _uow.Class.GetAllAsync()
        };

        return View(vm);
    }

    
    [HttpPost("editExam/{examId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int examId, EditExamVM vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Subjects = await _uow.Subject.GetAllAsync();
            vm.Classes = await _uow.Class.GetAllAsync();
            return View(vm);
        }

        string teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var exam = await _uow.Exam.FindAsync(
            e => e.ExamID == examId && e.TeacherID == teacherId);

        if (exam is null) return NotFound();

        exam.ExamType = vm.ExamType;
        exam.ExamDate = vm.ExamDate;
        exam.TotalMarks = vm.TotalMarks;
        exam.ExamDuration = vm.ExamDuration;
        exam.SubjectID = vm.SubjectId;
        exam.ClassID = vm.ClassId;
        exam.UpdatedAt = DateTime.UtcNow;

        await _uow.Exam.UpdateAsync(exam);
        await _uow.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    
    [HttpPost("deleteExam/{examId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int examId)
    {
        string teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var exam = await _uow.Exam.FindAsync(
            e => e.ExamID == examId && e.TeacherID == teacherId);

        if (exam is null) return NotFound();

        await _uow.Exam.DeleteAsync(exam);
        await _uow.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    
    [HttpGet("ExamResult/{examId:int}")]
    public async Task<IActionResult> ExamResult(int examId)
    {
        string teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var exam = await _uow.Exam.FindAsync(
            e => e.ExamID == examId && e.TeacherID == teacherId);

        if (exam is null) return NotFound();

        var results = await _uow.ExamResult.FindAllAsync(
            r => r.ExamId == examId,
            new[] { "Student.ApplicationUser" });

        var vm = results.Select(r => new ExamResultVM
        {
            ExamResultId = r.ExamResultID,
            StudentName = r.Student!.ApplicationUser!.FullName,
            Score = r.Score,
            Remarks = r.Remarks
        }).ToList();

        ViewBag.ExamInfo = $"{exam.ExamType} – {exam.Subject!.SubjectName}";
        return View(vm);
    }

    
    [HttpPost("ExamResult/{examResultId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Grade(int examResultId, GradeExamResultVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        string teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _uow.ExamResult.FindAsync(
            r => r.ExamResultID == examResultId && r.Exam!.TeacherID == teacherId,
            new[] { "Exam" });

        if (result is null) return NotFound();

        result.Score = vm.Score;
        result.Remarks = vm.Remarks;
        result.UpdatedAt = DateTime.UtcNow;

        await _uow.ExamResult.UpdateAsync(result);
        await _uow.SaveChangesAsync();

        return RedirectToAction(nameof(ExamResult), new { examId = result.ExamId });
    }
}

using System.Security.Claims;
using LMS.DataAccess.Repositories;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.TeacherArea.Controllers;

[Area("Teacher")]
[Authorize(Roles = SD.TeacherRole)]
public class ExamController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ExamController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: /Teacher/Exam
    [HttpGet]
        public async Task<IActionResult> Index()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var exams = await _unitOfWork.Exam
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
                        Status = e.ExamDate >= DateTime.UtcNow
                                        ? "Upcoming"
                                        : "Finished"
                    }).ToList()
                })
                .ToList();

            return View(vm);
        }

        // GET: /Teacher/Exam/uploadExam
        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var vm = new CreateExamVM
            {
                Subjects = await _unitOfWork.Subject.GetAllAsync() ?? new List<Subject>(),
                Classes = await _unitOfWork.Class.GetAllAsync() ?? new List<Class>()
            };
            return View(vm);
        }

        // POST: /Teacher/Exam/uploadExam
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(CreateExamVM vm)
        {
            // repopulate for redisplay
            vm.Subjects = await _unitOfWork.Subject.GetAllAsync() ?? new List<Subject>();
            vm.Classes = await _unitOfWork.Class.GetAllAsync() ?? new List<Class>();

            if (!ModelState.IsValid)
                return View(vm);

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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

            await _unitOfWork.Exam.AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Teacher/Exam/editExam/{examId}
        [HttpGet]
        public async Task<IActionResult> Edit(int examId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exam = await _unitOfWork.Exam.FindAsync(
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
                Subjects = await _unitOfWork.Subject.GetAllAsync() ?? new List<Subject>(),
                Classes = await _unitOfWork.Class.GetAllAsync() ?? new List<Class>()
            };

            return View(vm);
        }

        // POST: /Teacher/Exam/editExam/{examId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int examId, EditExamVM vm)
        {
            // repopulate for redisplay
            vm.Subjects = await _unitOfWork.Subject.GetAllAsync() ?? new List<Subject>();
            vm.Classes = await _unitOfWork.Class.GetAllAsync() ?? new List<Class>();

            if (!ModelState.IsValid)
                return View(vm);

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exam = await _unitOfWork.Exam.FindAsync(
                e => e.ExamID == examId && e.TeacherID == teacherId);

            if (exam is null) return NotFound();

            exam.ExamType = vm.ExamType;
            exam.ExamDate = vm.ExamDate;
            exam.TotalMarks = vm.TotalMarks;
            exam.ExamDuration = vm.ExamDuration;
            exam.SubjectID = vm.SubjectId;
            exam.ClassID = vm.ClassId;
            exam.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Exam.UpdateAsync(exam);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Teacher/Exam/deleteExam/{examId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int examId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exam = await _unitOfWork.Exam.FindAsync(
                e => e.ExamID == examId && e.TeacherID == teacherId);

            if (exam is null) return NotFound();

            await _unitOfWork.Exam.DeleteAsync(exam);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Teacher/ExamResult/{examId}
        [HttpGet]
        public async Task<IActionResult> ExamResult(int examId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // include Subject so it's never null
            var exam = await _unitOfWork.Exam.FindAsync(
                e => e.ExamID == examId && e.TeacherID == teacherId,
                new[] { "Subject" });

            if (exam is null) return NotFound();

            var results = await _unitOfWork.ExamResult.FindAllAsync(
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



    [HttpGet]
    public async Task<IActionResult> Grade(int examResultId)
    {
        var examResult = await _unitOfWork.ExamResult.FindAsync(s => s.ExamResultID == examResultId, ["Exam", "Student.ApplicationUser"]);
        var model = new GradeExamResultVM
        {
            ExamResultId = examResult.ExamResultID,
            ExamId = examResult.ExamId,
            StudentName = examResult.Student!.ApplicationUser!.FullName,
            Score = examResult.Score,
            Remarks = examResult.Remarks
        };
        return View();
    }

    // POST: /Teacher/ExamResult/{examResultId}
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Grade(int examResultId, GradeExamResultVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _unitOfWork.ExamResult.FindAsync(
                r => r.ExamResultID == examResultId && r.Exam!.TeacherID == teacherId,
                new[] { "Exam" });

            if (result is null) return NotFound();

            result.Score = vm.Score;
            result.Remarks = vm.Remarks;
            result.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ExamResult.UpdateAsync(result);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(ExamResult), new { examId = result.ExamId });
        }
    }


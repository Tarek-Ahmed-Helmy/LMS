using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.Web.Areas.StudentArea.Controllers;

[Area("Student")]
[Authorize(Roles = SD.StudentRole)]
public class QuizController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public QuizController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, ["ApplicationUser", "Class"]);

        if (student == null || student.ClassId == null)
            return NotFound("Student not found or not assigned to any class.");

        ViewBag.StudentName = student.ApplicationUser?.FullName ?? "N/A";

        var quizzes = await _unitOfWork.Exam.FindAllAsync(e => e.ClassID == student.ClassId && e.ExamType == ExamType.Quiz, includes: ["Subject"]);

        var submittedQuizIds = (await _unitOfWork.ExamResult.FindAllAsync(er => er.StudentID == id && er.Exam.ExamType == ExamType.Quiz)).Select(er => er.ExamId).ToHashSet();

        var quizViewModels = quizzes.Select(quiz => new AssignedQuizViewModel
        {
            ExamId = quiz.ExamID,
            SubjectName = quiz.Subject?.SubjectName ?? "N/A",
            ExamDate = quiz.ExamDate,
            TotalMarks = quiz.TotalMarks,
            IsSubmitted = submittedQuizIds.Contains(quiz.ExamID)
        }).ToList();

        return View(quizViewModels);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int quizId)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var exam = await _unitOfWork.Exam.FindAsync(e => e.ExamID == quizId && e.ExamType == ExamType.Quiz, includes: ["Subject", "Teacher.ApplicationUser"]);

        if (exam == null)
            return NotFound();

        var isSubmitted = (await _unitOfWork.ExamResult.FindAllAsync(er => er.ExamId == quizId && er.StudentID == id)).Any();

        var viewModel = new QuizDetailsViewModel
        {
            ExamId = exam.ExamID,
            SubjectName = exam.Subject?.SubjectName ?? "N/A",
            ExamDate = exam.ExamDate,
            TotalMarks = exam.TotalMarks,
            DurationInMinutes = exam.ExamDuration,
            TeacherName = exam.Teacher?.ApplicationUser?.FullName ?? "N/A",
            IsSubmitted = isSubmitted
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Submit(int quizId)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var questions = await _unitOfWork.QuizQuestion.FindAllAsync(q => q.ExamID == quizId);

        if (questions == null || !questions.Any())
            return NotFound("No quiz questions found.");

        var model = new SubmitQuizViewModel
        {
            QuizId = quizId,
            Questions = questions.Select(q => new QuizQuestionAnswer
            {
                QuestionId = q.QuizQuestionId,
                QuestionText = q.QuestionText,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Submit(SubmitQuizViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        int score = 0;
        foreach (var answer in model.Questions)
        {
            var question = await _unitOfWork.QuizQuestion.FindAsync(q => q.QuizQuestionId == answer.QuestionId);

            if (question == null) continue;

            var studentAnswer = new StudentAnswer
            {
                StudentId = id,
                QuizQuestionId = answer.QuestionId,
                SelectedAnswer = answer.SelectedAnswer
            };

            await _unitOfWork.StudentAnswer.AddAsync(studentAnswer);

            if (answer.SelectedAnswer == question.CorrectAnswer)
                score++;
        }

        var exam = await _unitOfWork.Exam.FindAsync(e => e.ExamID == model.QuizId);
        int totalMarks = exam?.TotalMarks ?? 10;
        int calculatedScore = (int)Math.Round((double)score / model.Questions.Count * totalMarks);

        var result = new ExamResult
        {
            ExamId = model.QuizId,
            StudentID = id,
            Score = calculatedScore
        };

        await _unitOfWork.ExamResult.AddAsync(result);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction("Feedback", new { quizId = model.QuizId });
    }

    [HttpGet]
    public async Task<IActionResult> Feedback(int quizId)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var exam = await _unitOfWork.Exam.FindAsync(e => e.ExamID == quizId, ["Subject"]);

        if (exam == null) return NotFound();

        var questions = await _unitOfWork.QuizQuestion.FindAllAsync(q => q.ExamID == quizId);

        var answers = await _unitOfWork.StudentAnswer.FindAllAsync(a => a.StudentId == id && questions.Select(q => q.QuizQuestionId).Contains(a.QuizQuestionId));

        var examResult = await _unitOfWork.ExamResult.FindAsync(r => r.ExamId == quizId && r.StudentID == id);

        var feedbackItems = questions.Select(q =>
        {
            var studentAnswer = answers.FirstOrDefault(a => a.QuizQuestionId == q.QuizQuestionId);
            return new QuizFeedbackItem
            {
                QuestionText = q.QuestionText,
                CorrectAnswer = q.CorrectAnswer,
                SelectedAnswer = studentAnswer?.SelectedAnswer ?? "Not Answered"
            };
        }).ToList();

        var viewModel = new QuizFeedbackViewModel
        {
            QuizId = quizId,
            SubjectName = exam.Subject?.SubjectName ?? "Unknown Subject",
            TotalMarks = exam.TotalMarks,
            StudentScore = examResult?.Score ?? 0,
            FeedbackItems = feedbackItems
        };

        return View(viewModel);
    }
}

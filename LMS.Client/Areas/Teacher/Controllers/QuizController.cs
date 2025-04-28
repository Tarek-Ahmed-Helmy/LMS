using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Web.Areas.TeacherArea.Controllers;

[Area("Teacher")]
[Authorize(Roles = SD.TeacherRole)]
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

        var quizzes = await _unitOfWork.Exam.FindAllAsync(q => q.TeacherID == id && q.ExamType == ExamType.Quiz, ["Subject", "Class"]);
        var result = quizzes.Select(q => new QuizListViewModel
        {
            ExamID = q.ExamID,
            SubjectName = q.Subject?.SubjectName ?? "N/A",
            ClassNumber = q.Class?.ClassNumber ?? "N/A",
            ExamDate = q.ExamDate,
            TotalMarks = q.TotalMarks,
            Duration = q.ExamDuration
        });

        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> AddQuiz()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var teacher = await _unitOfWork.Teacher.FindAsync(t => t.TeacherId == id, ["Schedules.Subject", "Schedules.Class"]);
        if (teacher == null || teacher.Schedules == null)
        {
            return NotFound("Teacher not found");
        }

        var model = new AddEditQuizViewModel
        {
            Subjects = teacher.Schedules.Where(s => s.Subject != null).Select(s => s.Subject).Distinct().ToList(),
            Classes = teacher.Schedules.Where(s => s.Class != null).Select(s => s.Class).Distinct().ToList(),
            Questions = new List<QuizQuestionViewModel>
            {
                new QuizQuestionViewModel()
            }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuiz(AddEditQuizViewModel model)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (ModelState.IsValid)
        {
            var quiz = new Exam
            {
                ExamDate = model.ExamDate,
                ExamType = ExamType.Quiz,
                TotalMarks = model.TotalMarks,
                ExamDuration = model.ExamDuration,
                SubjectID = model.SubjectID,
                ClassID = model.ClassID,
                TeacherID = id
            };

            await _unitOfWork.Exam.AddAsync(quiz);
            await _unitOfWork.SaveChangesAsync();


            foreach (var questionModel in model.Questions)
            {
                var quizQuestion = new QuizQuestion
                {
                    QuestionText = questionModel.QuestionText,
                    OptionA = questionModel.OptionA,
                    OptionB = questionModel.OptionB,
                    OptionC = questionModel.OptionC,
                    OptionD = questionModel.OptionD,
                    CorrectAnswer = questionModel.CorrectAnswer,
                    ExamID = quiz.ExamID
                };
                await _unitOfWork.QuizQuestion.AddAsync(quizQuestion);
            }

            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        var teacher = await _unitOfWork.Teacher.FindAsync(t => t.TeacherId == id, ["Schedules.Subject", "ApplicationUser"]);
        model.Subjects = teacher.Schedules.Where(s => s.Subject != null).Select(s => s.Subject).Distinct().ToList();
        model.Classes = teacher.Schedules.Where(s => s.Class != null).Select(s => s.Class).Distinct().ToList();
        model.Questions = new List<QuizQuestionViewModel>
            {
                new QuizQuestionViewModel()
            };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditQuiz(int quizId)
    {
        var quiz = await _unitOfWork.Exam.FindAsync(e => e.ExamID == quizId, ["quizQuestions", "Subject", "Class"]);
        if (quiz == null)
        {
            return NotFound("Quiz not found");
        }

        var model = new AddEditQuizViewModel
        {
            ExamID = quiz.ExamID,
            ExamDate = quiz.ExamDate,
            TotalMarks = quiz.TotalMarks,
            ExamDuration = quiz.ExamDuration,
            Subjects = quiz.Subject != null ? new List<Subject> { quiz.Subject } : new List<Subject>(),
            Classes = quiz.Class != null ? new List<Class> { quiz.Class } : new List<Class>(),
            Questions = quiz.quizQuestions.Select(q => new QuizQuestionViewModel
            {
                QuizQuestionId = q.QuizQuestionId,
                QuestionText = q.QuestionText,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                CorrectAnswer = q.CorrectAnswer
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditQuiz(int examID, AddEditQuizViewModel model)
    {
        if (ModelState.IsValid)
        {
            var quiz = await _unitOfWork.Exam.GetByIdAsync(examID);
            if (quiz == null)
            {
                return NotFound("Quiz not found");
            }

            quiz.ExamDate = model.ExamDate;
            quiz.TotalMarks = model.TotalMarks;
            quiz.ExamDuration = model.ExamDuration;

            await _unitOfWork.Exam.UpdateAsync(quiz);

            foreach (var questionModel in model.Questions)
            {
                if (questionModel.QuizQuestionId == null) // new question
                {
                    var newQuestion = new QuizQuestion
                    {
                        QuestionText = questionModel.QuestionText,
                        OptionA = questionModel.OptionA,
                        OptionB = questionModel.OptionB,
                        OptionC = questionModel.OptionC,
                        OptionD = questionModel.OptionD,
                        CorrectAnswer = questionModel.CorrectAnswer,
                        ExamID = quiz.ExamID
                    };
                    await _unitOfWork.QuizQuestion.AddAsync(newQuestion);
                }
                else // edit existing question
                {
                    var question = await _unitOfWork.QuizQuestion.GetByIdAsync(questionModel.QuizQuestionId.Value);
                    if (question != null)
                    {
                        question.QuestionText = questionModel.QuestionText;
                        question.OptionA = questionModel.OptionA;
                        question.OptionB = questionModel.OptionB;
                        question.OptionC = questionModel.OptionC;
                        question.OptionD = questionModel.OptionD;
                        question.CorrectAnswer = questionModel.CorrectAnswer;

                        await _unitOfWork.QuizQuestion.UpdateAsync(question);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // Redirect to quiz list after editing
        }
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var teacher = await _unitOfWork.Teacher.FindAsync(t => t.TeacherId == id, ["Schedules.Subject", "Schedules.Class"]);
        model.Subjects = teacher.Schedules.Select(s => s.Subject).ToList();
        model.Classes = teacher.Schedules.Select(s => s.Class).ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteQuiz(int quizId)
    {
        var quiz = await _unitOfWork.Exam.GetByIdAsync(quizId);
        if (quiz == null)
        {
            return NotFound("Quiz not found");
        }

        await _unitOfWork.Exam.DeleteAsync(quiz);
        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> QuizResults(int quizId)
    {
        var quizResults = await _unitOfWork.ExamResult.FindAllAsync(er => er.ExamId == quizId, ["Student.ApplicationUser"]);
        if (quizResults == null || !quizResults.Any())
        {
            return NotFound("No results found for this quiz");
        }

        var result = quizResults.Select(r => new QuizResultViewModel
        {
            StudentFullName = r.Student?.ApplicationUser.FullName,
            Score = r.Score
        }).ToList();

        return View(result);
    }

    [HttpGet]
    public IActionResult AddEmptyQuestion(int index)
    {
        var question = new QuizQuestionViewModel { Index = index };
        return PartialView("_QuizQuestionPartial", question);
    }

}
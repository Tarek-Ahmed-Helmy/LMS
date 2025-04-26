using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using LMS.Utilities;
using LMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Web.Areas.AdminArea.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class ParentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ParentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: /Parent/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET /Admin/Parent/GetParentList
        [HttpGet]
        public async Task<JsonResult> GetParentList()
        {
            var parents = await _unitOfWork.Parent
                .FindAllAsync(includes: new[] { "ApplicationUser" });

            var data = parents.Select(p => new {
                p.ParentId,
                FullName = p.ApplicationUser?.FullName,
                Email = p.ApplicationUser?.Email,
                Phone = p.ApplicationUser?.PhoneNumber,
                Occupation = p.Occupation,
                Status = (p.ApplicationUser?.LockoutEnd != null
                              && p.ApplicationUser.LockoutEnd > DateTimeOffset.UtcNow)
                              ? "Locked"
                              : "Active"
            }).ToList();

            return Json(new { data });
        }

        [HttpGet]
        public async Task<JsonResult> GetChildrenList(string id)
        {
            var students = await _unitOfWork.Student.FindAllAsync(s=>s.ParentId == id, ["ApplicationUser", "Class"]);
            var studentList = students.Select(s => new
            {
                s.StudentId,
                s.StudentNumber,
                s.ApplicationUser?.FullName,
                s.ApplicationUser?.Email,
                s.DateOfBirth,
                s.Gender,
                s.AdmissionDate,
                s.Class?.GradeLevel,
                s.Class?.ClassNumber

            }).ToList();
            return Json(new { data = studentList });
        }

        // GET: /Parent/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var parent = await _unitOfWork.Parent.FindAsync(p => p.ParentId == id, includes: new string[] { "ApplicationUser" });
            if (parent == null)
            {
                return NotFound();
            }

            var allStudents = await _unitOfWork.Student.FindAllAsync(s => s.ParentId == null, ["ApplicationUser"]);
            ViewBag.Students = allStudents
                .OrderBy(s => s.StudentNumber)
                .ThenBy(s => s.ApplicationUser.FullName)
                .Select(s => new SelectListItem
                {
                    Value = s.StudentId.ToString(),
                    Text = $"{s.StudentNumber} - {s.ApplicationUser.FullName}"
                }).ToList();

            var model = new ParentDetailsViewModel
            {
                ParentId = parent.ParentId,
                FullName = parent.ApplicationUser?.FullName,
                Address = parent.ApplicationUser?.Address,
                ProfilePictureURL = parent.ApplicationUser?.ProfilePictureURL,
                Email = parent.ApplicationUser?.Email,
                PhoneNumber = parent.ApplicationUser?.PhoneNumber, 
                Occupation = parent.Occupation
            };

            return View(model);
        }

        // GET: /Parent/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Parent/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ParentRegistrationViewModel newParent)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = newParent.Email,
                    Email = newParent.Email,
                    FullName = newParent.FullName,
                    Address = newParent.Address,
                    PhoneNumber = newParent.PhoneNumber 
                };
                await _userManager.AddToRoleAsync(user, SD.ParentRole);
                var result = await _userManager.CreateAsync(user, newParent.Password);
                if (result.Succeeded)
                {
                    var parent = new Parent
                    {
                        ParentId = user.Id,
                        Occupation = newParent.Occupation 
                    };

                    await _unitOfWork.Parent.AddAsync(parent);
                    await _unitOfWork.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(newParent);
        }

        // GET: /Parent/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var parent = await _unitOfWork.Parent.FindAsync(p => p.ParentId == id, includes: new string[] { "ApplicationUser" });
            if (parent == null)
            {
                return NotFound();
            }

            var viewModel = new ParentRegistrationViewModel
            {
                FullName = parent.ApplicationUser.FullName,
                Email = parent.ApplicationUser.Email,
                Address = parent.ApplicationUser.Address,
                PhoneNumber = parent.ApplicationUser.PhoneNumber, 
                Occupation = parent.Occupation 
            };

            return View(viewModel);
        }

        // POST: /Parent/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ParentRegistrationViewModel updatedParent)
        {
            if (!ModelState.IsValid)
                return View(updatedParent);

            var parent = await _unitOfWork.Parent.FindAsync(p => p.ParentId == id, includes: new string[] { "ApplicationUser" });
            if (parent == null)
            {
                return NotFound();
            }

            parent.ApplicationUser.FullName = updatedParent.FullName;
            parent.ApplicationUser.Email = updatedParent.Email;
            parent.ApplicationUser.UserName = updatedParent.Email;
            parent.ApplicationUser.Address = updatedParent.Address;
            parent.ApplicationUser.PhoneNumber = updatedParent.PhoneNumber; 
            parent.Occupation = updatedParent.Occupation;

            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Parent/ChangeStatus/{id}
        [HttpGet]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (!user.LockoutEnabled)
            {
                user.LockoutEnabled = true;
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                TempData["StatusMessage"] = "Parent account has been unlocked.";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData["StatusMessage"] = "Parent account has been locked.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToParent(ParentDetailsViewModel model)
        {
            var students = await _unitOfWork.Student.FindAllAsync(s => model.SelectedStudentsIds.Contains(s.StudentId));
            if (students == null || !students.Any())
            {
                return NotFound();
            }
            foreach (var student in students)
            {
                student.ParentId = model.ParentId;
            }

            await _unitOfWork.Student.UpdateRangeAsync(students);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = model.ParentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromParent(string studentId, string parentId)
        {
            var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                return NotFound();
            }
            student.ParentId = null;
            await _unitOfWork.Student.UpdateAsync(student);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = parentId });
        }
    }
}

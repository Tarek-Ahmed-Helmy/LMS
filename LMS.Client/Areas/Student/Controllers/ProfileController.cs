using LMS.Entities.Interfaces;
using LMS.Web.ViewModels.Student;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Web.Areas.Student.Controllers;

[Area("Student")]
public class ProfileController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ProfileController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Profile(string id)
    {
        var student = await _unitOfWork.Student.FindAsync(s => s.StudentId == id, ["ApplicationUser", "Class"]);
        if (student == null)
        {
            return NotFound();
        }
        var studentProfileVM = new StudentProfileViewModel()
        {
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            EmergencyContact = student.EmergencyContact,
            AdmissionDate = student.AdmissionDate,
            StudentNumber = student.StudentNumber,
            FullName = student.ApplicationUser.FullName,
            Address = student.ApplicationUser.Address,
            ProfilePictureURL = student.ApplicationUser.ProfilePictureURL,
            GradeLevel = student.Class.GradeLevel,
            ClassNumber = student.Class.ClassNumber,
            BusId = student.BusId
        };
        return View(student);
    }
}

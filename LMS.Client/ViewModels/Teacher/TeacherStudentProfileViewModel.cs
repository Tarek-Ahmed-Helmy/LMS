using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels.Teacher;


public class TeacherStudentProfileViewModel
{
    // Profile Info
    [Display(Name = "Student ID")]
    public string StudentId { get; set; }

    [Display(Name = "Full Name")]
    [Required(ErrorMessage = "Full name is required.")]
    public string FullName { get; set; }

    [Display(Name = "Student Number")]
    [Required(ErrorMessage = "Student number is required.")]
    public string StudentNumber { get; set; }

    [Display(Name = "Class")]
    public string ClassName { get; set; }

    [Display(Name = "Date of Birth")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime DateOfBirth { get; set; }

    [Display(Name = "Gender")]
    public string Gender { get; set; }

    [Display(Name = "Emergency Contact")]
    [Required(ErrorMessage = "Emergency contact is required.")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    public string EmergencyContact { get; set; }

    [Display(Name = "Admission Date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime AdmissionDate { get; set; }

    [Display(Name = "Profile Picture")]
    public string ProfilePictureURL { get; set; }

    // Performance Summary
    [Display(Name = "Attendance Percentage")]
    [DisplayFormat(DataFormatString = "{0:F2}%")]
    public double? AttendancePercentage { get; set; }

    [Display(Name = "Average Exam Score")]
    [DisplayFormat(DataFormatString = "{0:F2}")]
    public double AverageExamScore { get; set; }

    [Display(Name = "Average Assignment Score")]
    [DisplayFormat(DataFormatString = "{0:F2}")]
    public double AverageAssignmentScore { get; set; }

    // Detailed Performance
    [Display(Name = "Recent Attendance")]
    public List<AttendanceViewModel> RecentAttendances { get; set; }

    [Display(Name = "Exam Results")]
    public List<ExamResultViewModel> ExamResults { get; set; }

    [Display(Name = "Assignment Submissions")]
    public List<SubmissionViewModel> Submissions { get; set; }
}

public class AttendanceViewModel
{
    [Display(Name = "Date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime Date { get; set; }

    [Display(Name = "Status")]
    public string Status { get; set; }

    [Display(Name = "Notes")]
    public string Notes { get; set; }
}

public class ExamResultViewModel
{
    [Display(Name = "Exam ID")]
    public int ExamId { get; set; }

    [Display(Name = "Exam Type")]
    public string ExamType { get; set; }

    [Display(Name = "Subject")]
    public string SubjectName { get; set; }

    [Display(Name = "Score")]
    public int Score { get; set; }

    [Display(Name = "Total Marks")]
    public int TotalMarks { get; set; }

    [Display(Name = "Remarks")]
    public string Remarks { get; set; }
}

public class SubmissionViewModel
{
    [Display(Name = "Assignment ID")]
    public int AssignmentId { get; set; }

    [Display(Name = "Assignment Title")]
    public string AssignmentTitle { get; set; }

    [Display(Name = "Submission Date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime SubmissionDate { get; set; }

    [Display(Name = "Score")]
    public int Score { get; set; }

    [Display(Name = "Total Marks")]
    public int TotalMarks { get; set; }

    [Display(Name = "Feedback")]
    public string Feedback { get; set; }
}

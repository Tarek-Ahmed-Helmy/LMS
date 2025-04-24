using LMS.Web.ViewModels;
using System.ComponentModel.DataAnnotations;
namespace LMS.Web.ViewModels;

public class EditExamVM : CreateExamVM
{
    public int ExamId { get; set; }
}
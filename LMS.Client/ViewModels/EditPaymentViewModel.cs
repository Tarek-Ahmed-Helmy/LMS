using LMS.Entities.Models;
using LMS.Web.ViewModels;
namespace LMS.Web.ViewModels;

public class EditPaymentViewModel : CreatePaymentViewModel
{
    internal IEnumerable<Student> Students;
    internal IEnumerable<Parent> Parents;

    public int PaymentId { get; set; }
}
using LMS.Entities.Models;

namespace LMS.Web.ViewModels.AdminViewModels;

public class ClassManagementViewModel
{
    public IEnumerable<Class>? Classes { get; set; }
    public ClassViewModel NewClass { get; set; }
}

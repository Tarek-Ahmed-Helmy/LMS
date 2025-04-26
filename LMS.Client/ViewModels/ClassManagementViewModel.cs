using LMS.Entities.Models;

namespace LMS.Web.ViewModels;

public class ClassManagementViewModel
{
    public IEnumerable<Class>? Classes { get; set; }
    public ClassViewModel NewClass { get; set; }
}

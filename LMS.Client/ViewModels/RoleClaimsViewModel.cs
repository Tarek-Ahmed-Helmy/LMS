using LMS.Utilities;

namespace LMS.Web.ViewModels;

public class RoleClaimsViewModel
{
    public RoleClaimsViewModel()
    {
        Claims = new List<AppClaim>();
    }
    public string RoleId { get; set; }
    public List<AppClaim> Claims { get; set; }
}

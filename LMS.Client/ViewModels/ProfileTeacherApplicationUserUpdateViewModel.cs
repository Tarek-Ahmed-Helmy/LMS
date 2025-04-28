namespace LMS.Web.ViewModels;

public class ProfileTeacherApplicationUserUpdateViewModel
{
    public string FullName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? ProfilePictureURL { get; set; }
    public DateTime UpdatedAt { get; set; }
}

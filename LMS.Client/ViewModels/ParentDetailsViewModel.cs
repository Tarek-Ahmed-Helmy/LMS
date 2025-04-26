namespace LMS.Web.ViewModels;

public class ParentDetailsViewModel
{
    public string? ParentId { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public string ProfilePictureURL { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }  // Updated to use PhoneNumber
    public string Occupation { get; set; }
    public List<string>? SelectedStudentsIds { get; set; } = new List<string>();
}

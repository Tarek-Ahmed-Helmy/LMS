namespace LMS.Web.ViewModels;

public class BusDetailsViewModel
{
    public int BusId { get; set; }
    public string Route { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string DriverContact { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int StudentCount { get; set; }
    public List<StudentBusViewModel> Students { get; set; } = new List<StudentBusViewModel>();
    public List<string>? SelectedStudentsIds { get; set; } = new List<string>();
}

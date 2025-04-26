namespace LMS.Web.ViewModels;

public class BusListViewModel
{
    public int BusId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Route { get; set; } = string.Empty;
    public string DriverContact { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}

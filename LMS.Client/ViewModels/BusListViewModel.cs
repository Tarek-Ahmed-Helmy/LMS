using LMS.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels.AdminViewModels;

public class BusListViewModel
{
    public int BusId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Route { get; set; } = string.Empty;
    public string DriverContact { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public List<ClassSummaryViewModel> Classes { get; set; } = new List<ClassSummaryViewModel>();
}

public class RouteListViewModel
{
    public string RouteName { get; set; } = string.Empty;
    public List<BusSummaryViewModel> Buses { get; set; } = new List<BusSummaryViewModel>();
}

public class BusSummaryViewModel
{
    public int BusId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int StudentCount { get; set; }
    public List<ClassSummaryViewModel> Classes { get; set; } = new List<ClassSummaryViewModel>();
}

public class BusScheduleViewModel
{
    public BusDetailsViewModel Bus { get; set; } = new BusDetailsViewModel();
    public List<ScheduleDetailsViewModel> Schedules { get; set; } = new List<ScheduleDetailsViewModel>();
}

public class BusDetailsViewModel
{
    public int BusId { get; set; }
    public string Route { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int StudentCount { get; set; }
}

public class ScheduleDetailsViewModel
{
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string? DriverId { get; set; }
}

public class ClassSummaryViewModel
{
    public int ClassId { get; set; }
    public GradeLevel GradeLevel { get; set; }
    public string? ClassNumber { get; set; }
    public int StudentCount { get; set; }
}

public class CreateBusViewModel
{
    [Required]
    [StringLength(255)]
    public string DriverName { get; set; } = string.Empty;

    [Required]
    public string Route { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
    public int Capacity { get; set; }

    [Required]
    [StringLength(20)]
    public string DriverContact { get; set; } = string.Empty;
}
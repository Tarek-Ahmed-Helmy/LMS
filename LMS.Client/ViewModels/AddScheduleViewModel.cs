using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.ViewModels;

public class AddScheduleViewModel
{
    [Required(ErrorMessage = "Please select a Teacher")]
    [Display(Name = "Teacher")]
    public string? TeacherId { get; set; }

    [Required(ErrorMessage = "Please select a day")]
    public DayOfWeek DayOfWeek { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; }

    [Required(ErrorMessage = "Please select a class")]
    [Display(Name = "Class")]
    public int ClassId { get; set; }



    [Required(ErrorMessage = "Please select a subject")]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }


}
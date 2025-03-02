using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Entities.Models;

public class Attendance
{
    [Key]
    public int AttendanceID { get; set; }

    [ForeignKey("ApplicationUser")]
    public int UserID { get; set; }
    public DateTime? Date { get; set; }

    [Required]
    [StringLength(50)]
    [RegularExpression("Present|Absent|Late", ErrorMessage = "Status must be 'Present', 'Absent', or 'Late'.")]
    public string Status { get; set; }

    [Required]
    [StringLength(255)]
    public string? Remarks { get; set; }
}

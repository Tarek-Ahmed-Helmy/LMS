using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Entities.Models;

public class ExamResults
{
    [Key]
    public int ResultID { get; set; }

    [ForeignKey("Exams")]
    public int ExamId { get; set; }

    [ForeignKey("Students")]    
    public int StudentID { get; set; }
    public int Marks { get; set; }

    [Required]
    [StringLength(5)]
    public string Grade { get; set; }
}

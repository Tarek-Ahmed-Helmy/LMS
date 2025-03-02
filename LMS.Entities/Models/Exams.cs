using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Entities.Models;

public class Exams
{
    [Key]
    public int ExamID { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [ForeignKey("Subjects")]
    public int SubjectID { get; set; }

    [ForeignKey("Class")]
    public int ClassID { get; set; }

    public DateTime ExamDate { get; set; }
}

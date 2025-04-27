namespace LMS.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class GradeSubmissionViewModel
    {
        public int SubmissionId { get; set; }

        public string AssignmentTitle { get; set; } = string.Empty;
        public string AssignmentDescription { get; set; } = string.Empty;

        [Required, Range(0, 100)]
        public int ? Score { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Feedback { get; set; }
    }
}

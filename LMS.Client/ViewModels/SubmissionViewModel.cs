namespace LMS.Web.ViewModels
{
    public class SubmissionViewModel
    {
        public int SubmissionId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime SubmissionDate { get; set; }
        public int Score { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }
}
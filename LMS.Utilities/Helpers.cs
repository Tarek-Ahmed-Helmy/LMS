using LMS.Entities.Models;

namespace LMS.Utilities;

public static class Helpers
{
    public static string GetSubmissionProgress(Submission? submission, int totalMarks, out string grade)
    {
        string progress;
        grade = "--";

        if (submission == null)
        {
            progress = "Not Submitted";
        }
        else if (submission.Score >= 0)
        {
            progress = "Graded";
            grade = $"{submission.Score} / {totalMarks}";
        }
        else
        {
            progress = "Pending Grading";
        }

        return progress;
    }
}

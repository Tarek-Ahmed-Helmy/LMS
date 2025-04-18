namespace LMS.Entities.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IApplicationUserRepository ApplicationUser { get; }
    IAssignmentRepository Assignment { get; }
    IAttendanceRepository Attendance { get; }
    IAttendeeRepository Attendee { get; }
    IBusRepository Bus { get; }
    IClassRepository Class { get; }
    IEventRepository Event { get; }
    IExamRepository Exam { get; }
    IExamResultRepository ExamResult { get; }
    IMeetingRepository Meeting { get; }
    INotificationRepository Notification { get; }
    IParentRepository Parent { get; }
    IPaymentRepository Payment { get; }
    IResourceRepository Resource { get; }
    IScheduleRepository Schedule { get; }
    IStudentRepository Student { get; }
    ISubjectRepository Subject { get; }
    ISubmissionRepository Submission { get; }
    ITeacherRepository Teacher { get; }
    IQuizQuestionRepository QuizQuestion { get; }
    IStudentAnswerRepository StudentAnswer { get; }
    Task<int> SaveChangesAsync();
}

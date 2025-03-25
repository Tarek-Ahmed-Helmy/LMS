namespace LMS.Entities.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IApplicationUserRepository ApplicationUserRepository { get; }
    IAssignmentRepository AssignmentRepository { get; }
    IAttendanceRepository AttendanceRepository { get; }
    IAttendeeRepository AttendeeRepository { get; }
    IBusRepository BusRepository { get; }
    IClassRepository ClassRepository { get; }
    IEventRepository EventRepository { get; }
    IExamRepository ExamRepository { get; }
    IExamResultRepository ExamResultRepository { get; }
    IMeetingRepository MeetingRepository { get; }
    INotificationRepository NotificationRepository { get; }
    IParentRepository ParentRepository { get; }
    IPaymentRepository PaymentRepository { get; }
    IResourceRepository ResourceRepository { get; }
    IScheduleRepository ScheduleRepository { get; }
    IStudentRepository StudentRepository { get; }
    ISubjectRepository SubjectRepository { get; }
    ISubmissionRepository SubmissionRepository { get; }
    ITeacherRepository TeacherRepository { get; }
    Task<int> SaveChangesAsync();
}

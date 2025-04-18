using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.DataAccess.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IApplicationUserRepository ApplicationUser { get; }
    public IAssignmentRepository Assignment { get; }
    public IAttendanceRepository Attendance { get; }
    public IAttendeeRepository Attendee { get; }
    public IBusRepository Bus { get; }
    public IClassRepository Class { get; }
    public IEventRepository Event { get; }
    public IExamRepository Exam { get; }
    public IExamResultRepository ExamResult { get; }
    public IMeetingRepository Meeting { get; }
    public INotificationRepository Notification { get; }
    public IParentRepository Parent { get; }
    public IPaymentRepository Payment { get; }
    public IResourceRepository Resource { get; }
    public IScheduleRepository Schedule { get; }
    public IStudentRepository Student { get; }
    public ISubjectRepository Subject { get; }
    public ISubmissionRepository Submission { get; }
    public ITeacherRepository Teacher { get; }
    public IQuizQuestionRepository QuizQuestion { get; }
    public IStudentAnswerRepository StudentAnswer { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        ApplicationUser = new ApplicationUserRepository(_context);
        Assignment = new AssignmentRepository(_context);
        Attendance = new AttendanceRepository(_context);
        Attendee = new AttendeeRepository(_context);
        Bus = new BusRepository(_context);
        Class = new ClassRepository(_context);
        Event = new EventRepository(_context);
        Exam = new ExamRepository(_context);
        ExamResult = new ExamResultRepository(_context);
        Meeting = new MeetingRepository(_context);
        Notification = new NotificationRepository(_context);
        Parent = new ParentRepository(_context);
        Payment = new PaymentRepository(_context);
        Resource = new ResourceRepository(_context);
        Schedule = new ScheduleRepository(_context);
        Student = new StudentRepository(_context);
        Subject = new SubjectRepository(_context);
        Submission = new SubmissionRepository(_context);
        Teacher = new TeacherRepository(_context);
        QuizQuestion = new QuizQuestionRepository(_context);
        StudentAnswer = new StudentAnswerRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Modified)
            {
                var updatedAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
                if (updatedAtProperty is not null)
                {
                    updatedAtProperty.CurrentValue = DateTime.UtcNow;
                }
            }
        }
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.DataAccess.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IApplicationUserRepository ApplicationUserRepository { get; }
    public IAssignmentRepository AssignmentRepository { get; }
    public IAttendanceRepository AttendanceRepository { get; }
    public IAttendeeRepository AttendeeRepository { get; }
    public IBusRepository BusRepository { get; }
    public IClassRepository ClassRepository { get; }
    public IEventRepository EventRepository { get; }
    public IExamRepository ExamRepository { get; }
    public IExamResultRepository ExamResultRepository { get; }
    public IMeetingRepository MeetingRepository { get; }
    public INotificationRepository NotificationRepository { get; }
    public IParentRepository ParentRepository { get; }
    public IPaymentRepository PaymentRepository { get; }
    public IResourceRepository ResourceRepository { get; }
    public IScheduleRepository ScheduleRepository { get; }
    public IStudentRepository StudentRepository { get; }
    public ISubjectRepository SubjectRepository { get; }
    public ISubmissionRepository SubmissionRepository { get; }
    public ITeacherRepository TeacherRepository { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        ApplicationUserRepository = new ApplicationUserRepository(_context);
        AssignmentRepository = new AssignmentRepository(_context);
        AttendanceRepository = new AttendanceRepository(_context);
        AttendeeRepository = new AttendeeRepository(_context);
        BusRepository = new BusRepository(_context);
        ClassRepository = new ClassRepository(_context);
        EventRepository = new EventRepository(_context);
        ExamRepository = new ExamRepository(_context);
        ExamResultRepository = new ExamResultRepository(_context);
        MeetingRepository = new MeetingRepository(_context);
        NotificationRepository = new NotificationRepository(_context);
        ParentRepository = new ParentRepository(_context);
        PaymentRepository = new PaymentRepository(_context);
        ResourceRepository = new ResourceRepository(_context);
        ScheduleRepository = new ScheduleRepository(_context);
        StudentRepository = new StudentRepository(_context);
        SubjectRepository = new SubjectRepository(_context);
        SubmissionRepository = new SubmissionRepository(_context);
        TeacherRepository = new TeacherRepository(_context);
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

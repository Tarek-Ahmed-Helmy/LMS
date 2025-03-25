using LMS.DataAccess.Data;
using LMS.DataAccess.Repositories;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.DataAccess.DependencyInjection;

public static class DataAccessServiceRegistration
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Connection")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IAttendeeRepository, AttendeeRepository>();
        services.AddScoped<IBusRepository, BusRepository>();
        services.AddScoped<IClassRepository, ClassRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IExamResultRepository, ExamResultRepository>();
        services.AddScoped<IMeetingRepository, MeetingRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IParentRepository, ParentRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();

        return services;
    }
}

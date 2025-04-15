using LMS.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<IdentityUserRole<string>>()
               .HasOne<IdentityRole>()
               .WithMany()
               .HasForeignKey(ur => ur.RoleId)
               .OnDelete(DeleteBehavior.NoAction);
    }

    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Attendee> Attendees { get; set; }
    public DbSet<Bus> Buses { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamResult> ExamResults { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
}

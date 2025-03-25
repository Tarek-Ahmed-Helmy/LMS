using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class AttendeeRepository : GenericRepository<Attendee>, IAttendeeRepository
{
    public AttendeeRepository(ApplicationDbContext context) : base(context)
    {
    }
}

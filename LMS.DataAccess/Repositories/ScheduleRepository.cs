using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
{
    public ScheduleRepository(ApplicationDbContext context) : base(context)
    {
    }
}

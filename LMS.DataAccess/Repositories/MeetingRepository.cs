using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class MeetingRepository : GenericRepository<Meeting>, IMeetingRepository
{
    public MeetingRepository(ApplicationDbContext context) : base(context)
    {
    }
}

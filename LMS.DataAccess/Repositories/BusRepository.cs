using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class BusRepository : GenericRepository<Bus>, IBusRepository
{
    public BusRepository(ApplicationDbContext context) : base(context)
    {
    }
}

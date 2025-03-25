using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class ClassRepository : GenericRepository<Class>, IClassRepository
{
    public ClassRepository(ApplicationDbContext context) : base(context)
    {
    }
}

using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class ParentRepository : GenericRepository<Parent>, IParentRepository
{
    public ParentRepository(ApplicationDbContext context) : base(context)
    {
    }
}

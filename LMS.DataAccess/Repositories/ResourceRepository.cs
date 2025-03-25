using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class ResourceRepository : GenericRepository<Resource>, IResourceRepository
{
    public ResourceRepository(ApplicationDbContext context) : base(context)
    {
    }
}

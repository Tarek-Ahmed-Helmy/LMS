using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
{
    public ApplicationUserRepository(ApplicationDbContext context) : base(context)
    {
    }
}

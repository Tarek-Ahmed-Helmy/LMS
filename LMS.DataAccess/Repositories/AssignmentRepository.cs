using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(ApplicationDbContext context) : base(context)
    {
    }
}

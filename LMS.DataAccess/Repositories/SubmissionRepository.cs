using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class SubmissionRepository : GenericRepository<Submission>, ISubmissionRepository
{
    public SubmissionRepository(ApplicationDbContext context) : base(context)
    {
    }
}

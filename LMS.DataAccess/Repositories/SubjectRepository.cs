using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(ApplicationDbContext context) : base(context)
    {
    }
}
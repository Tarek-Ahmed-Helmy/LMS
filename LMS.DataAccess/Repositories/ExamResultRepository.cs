using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class ExamResultRepository : GenericRepository<ExamResult>, IExamResultRepository
{
    public ExamResultRepository(ApplicationDbContext context) : base(context)
    {
    }
}

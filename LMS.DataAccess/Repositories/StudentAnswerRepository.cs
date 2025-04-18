using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class StudentAnswerRepository : GenericRepository<StudentAnswer>, IStudentAnswerRepository
{
    public StudentAnswerRepository(ApplicationDbContext context) : base(context)
    {
    }
}

using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class QuizQuestionRepository : GenericRepository<QuizQuestion>, IQuizQuestionRepository
{
    public QuizQuestionRepository(ApplicationDbContext context) : base(context)
    {
    }
}

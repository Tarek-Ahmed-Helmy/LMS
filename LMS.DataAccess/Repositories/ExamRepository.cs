using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LMS.DataAccess.Repositories;

public class ExamRepository : GenericRepository<Exam>, IExamRepository
{
    private readonly ApplicationDbContext _context;
    public ExamRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Exam>> FindExamScheduleAsync(Expression<Func<Exam, bool>> criteria, DateTime? fromDate, DateTime? toDate, string[] includes)
    {
        var examsQuery = _context.Exams.Where(criteria);

        if (fromDate.HasValue)
            examsQuery = examsQuery.Where(e => e.ExamDate >= fromDate.Value.Date);
        if (toDate.HasValue)
            examsQuery = examsQuery.Where(e => e.ExamDate <= toDate.Value.Date);

        foreach (var include in includes)
            examsQuery = examsQuery.Include(include);

        return await examsQuery.ToListAsync();
    }
}

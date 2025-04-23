using LMS.Entities.Models;
using System.Linq.Expressions;

namespace LMS.Entities.Interfaces;

public interface IExamRepository : IGenericRepository<Exam>
{
    Task<IEnumerable<Exam>> FindExamScheduleAsync(Expression<Func<Exam, bool>> criteria, DateTime? fromDate, DateTime? toDate, string[] includes);
}

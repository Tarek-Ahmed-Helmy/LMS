using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using LMS.Entities.Models;

namespace LMS.DataAccess.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context)
    {
    }
}

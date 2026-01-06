using Covenant.Common.Entities.Notification;
using Covenant.Common.Enums;
using Covenant.Common.Models.Notification;
using Covenant.Common.Repositories.Notification;
using Covenant.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories.Notification
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly CovenantContext _context;

        public NotificationRepository(CovenantContext context) => _context = context;

        public Task<UserNotificationType> Get(Guid userId, int typeId) =>
            _context.UserNotificationType.SingleOrDefaultAsync(s => s.UserId == userId
                                                                    && s.NotificationTypeId == typeId);

        public Task<List<UserNotificationListModel>> Get(Guid userId, IEnumerable<NotificationTarget> targetAllow)
        {
            return (from n in _context.NotificationType.Where(t => targetAllow.Contains(t.Target))
                    join ust in _context.UserNotificationType.Where(t => t.UserId == userId) on n.Id equals ust.NotificationTypeId
                        into tmp
                    from ust in tmp.DefaultIfEmpty()
                    select new UserNotificationListModel
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Description = n.Description,
                        EmailNotification = ust != null && ust.EmailNotification,
                        PushNotification = ust != null && ust.PushNotification,
                        SMSNotification = ust != null && ust.SMSNotification
                    }).ToListAsync();
        }

        public async Task Create(UserNotificationType entity) => await _context.UserNotificationType.AddAsync(entity);

        public Task Update(UserNotificationType entity) => Task.FromResult(_context.UserNotificationType.Update(entity));

        public async Task CreateUpdate(Guid userId, UserNotificationUpdateModel model)
        {
            UserNotificationType entity = await _context.UserNotificationType.SingleOrDefaultAsync(un => un.UserId == userId && un.NotificationTypeId == model.Id);
            if (entity is null)
            {
                entity = new UserNotificationType(userId, model.Id)
                { EmailNotification = model.EmailNotification, PushNotification = model.PushNotification, SMSNotification = model.SMSNotification };
                await _context.UserNotificationType.AddAsync(entity);
            }
            else
            {
                entity.EmailNotification = model.EmailNotification;
                entity.PushNotification = model.PushNotification;
                entity.SMSNotification = model.SMSNotification;
                _context.UserNotificationType.Update(entity);
            }
        }

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Common.Notifications;

namespace BabyTurismo.Application.Common.Interfaces;

public interface INotificationRepository : IRepository<Notification>
{
    Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(Guid? userId, string[] roles, CancellationToken cancellationToken = default);
}

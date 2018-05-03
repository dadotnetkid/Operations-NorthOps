using System.Threading.Tasks;

namespace NorthOps.Services.NotificationService
{
    public interface INotificationService
    {
        Task Notify(string userId, string message);
    }
}
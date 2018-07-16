using System.Threading.Tasks;
using NorthOps.Models;

namespace NorthOps.Services.EmailService
{
    public interface IEmailServices
    {
        Task Send(string userId, string subject,NotificationType notificationType);
        Task Send(string userId, NotificationType notificationType);
        
    }
}
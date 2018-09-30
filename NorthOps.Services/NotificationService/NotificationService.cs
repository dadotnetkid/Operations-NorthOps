using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Services.NotificationService;

namespace NorthOps.Services.NotificationService
{
    public class NotificationService
    {
        private INotificationService notificationService;
        private UnitOfWork unitOfWork = new UnitOfWork();

        public NotificationService(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public async Task NotifyPersonalInterviewStatus(string userId, bool isPassed)
        {

            if (isPassed)
            {
                var Template = await
                    unitOfWork.NotificationTemplatesRepo.FindAsync(m => m.Type == (int)NotificationType.IsPersonalInterviewPassed);
                await this.notificationService.Notify(userId, Template.Template);
            }
            else
            {
                var Template = await
                    unitOfWork.NotificationTemplatesRepo.FindAsync(m => m.Type == (int)NotificationType.IsPersonalInterviewFailed);
                await this.notificationService.Notify(userId, Template.Template);
            }
        }

        public async Task NotifyPhoneInterviewStatus(string userId, bool isPassed)
        {

        }
    }
}

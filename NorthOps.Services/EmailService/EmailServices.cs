using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.AspIdentity;
using NorthOps.Models;

namespace NorthOps.Services.NotificationService.EmailService
{
    public class EmailServices : IEmailServices
    {
        private IEmailServices emailService;

        public EmailServices(IEmailServices emailService)
        {
            this.emailService = emailService;
        }
        public async Task Send(string userId, string subject, NotificationType notificationType)
        {
            await emailService.Send(userId, subject, notificationType);
        }

        public async Task SendApplicantStatus(string userId, bool isPassed)
        {
            if (isPassed)
                await emailService.Send(userId, notificationType: NotificationType.IsPersonalInterviewPassed);
            else
                await emailService.Send(userId, notificationType: NotificationType.IsPersonalInterviewFailed);

        }

        public async Task Send(string userId, NotificationType notificationType)
        {
            await emailService.Send(userId, notificationType);
        }

    }
}

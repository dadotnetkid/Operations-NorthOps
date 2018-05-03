using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.AspIdentity;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Services.NotificationService.EmailService
{
    public class ApplicantStatusEmailService : IEmailServices
    {
        private ApplicationUserManager userManager;
        private JobApplications jobApplications;
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ApplicantStatusEmailService(ApplicationUserManager userManager, JobApplications jobApplications)
        {
            this.userManager = userManager;
            this.jobApplications = jobApplications;
        }
        public async Task Send(string userId, string subject, NotificationType notificationType)
        {
            throw new NotImplementedException();
        }

        public async Task Send(string userId, NotificationType notificationType)
        {
            var emailTemplate = unitOfWork.NotificationTemplatesRepo.Find(m => m.Type == (int)notificationType)?.Template;
            await userManager.SendEmailAsync(userId, "NorthOps", emailTemplate);
        }
    }
}

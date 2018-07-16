using System;
using System.Threading.Tasks;
using NorthOps.AspIdentity;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Services.EmailService
{
    public class RecruitmentEmailService : IEmailServices
    {
        private IEmailServices emailService;
        private UnitOfWork unitOfWork = new UnitOfWork();
        private ApplicationUserManager userManager;
        private JobApplications jobApplications;

        public RecruitmentEmailService(ApplicationUserManager userManager, JobApplications jobApplications)
        {
            this.userManager = userManager;
            this.jobApplications = jobApplications;
        }
        public async Task Send(string userId, string subject, NotificationType notificationType)
        {
            var emailTemplate = unitOfWork.NotificationTemplatesRepo.Find(m => m.Type == (int)notificationType)?.Template;
            switch (notificationType)
            {
                case NotificationType.Resume:
                    break;
                case NotificationType.PhoneInterview:
                    emailTemplate = emailTemplate?
                        .Replace("@Date", this.jobApplications.PhoneInterviewDate?.ToString("MM/dd/yy"))
                        .Replace("@TimeFrom", this.jobApplications.PhoneInterviewDate?.ToString("hh:mm tt"))
                        .Replace("@TimeTo", this.jobApplications.PhoneInterviewDate?.AddHours(2).ToString("hh:mm tt"));
                    break;
                case NotificationType.PersonalInterview:
                    emailTemplate = emailTemplate?
                        .Replace("@Date", this.jobApplications.PersonalInterviewDate?.ToString("MM/dd/yy"))
                        .Replace("@TimeFrom", this.jobApplications.PersonalInterviewDate?.ToString("hh:mm tt"))
                        .Replace("@TimeTo", this.jobApplications.PersonalInterviewDate?.AddHours(2).ToString("hh:mm tt"));
                    break;
                case NotificationType.Training:
                    emailTemplate = emailTemplate?
                        .Replace("@Date", this.jobApplications.TrainingDate?.ToString("MM/dd/yy"))
                        .Replace("@TimeFrom", this.jobApplications.TrainingDate?.ToString("hh:mm tt"))
                        .Replace("@TimeTo", this.jobApplications.TrainingDate?.AddHours(2).ToString("hh:mm tt"));
                    break;
                case NotificationType.OnBoarding:
                    emailTemplate = emailTemplate?
                        .Replace("@Date", this.jobApplications.OnBoardingDate?.ToString("MM/dd/yy"))
                        .Replace("@TimeFrom", this.jobApplications.OnBoardingDate?.ToString("hh:mm tt"))
                        .Replace("@TimeTo", this.jobApplications.OnBoardingDate?.AddHours(2).ToString("hh:mm tt"));
                    break;
                case NotificationType.Contract:
                    emailTemplate = emailTemplate?
                        .Replace("@Date", this.jobApplications.ContractDate?.ToString("MM/dd/yy"))
                        .Replace("@TimeFrom", this.jobApplications.ContractDate?.ToString("hh:mm tt"))
                        .Replace("@TimeTo", this.jobApplications.ContractDate?.AddHours(2).ToString("hh:mm tt"));
                    break;

                case NotificationType.IsExamFailed:
                    

                case NotificationType.IsPersonalInterviewPassed:
                

                    break;
                default:
                    break;
            }
            await userManager.SendEmailAsync(userId, subject, emailTemplate);
        }

        public Task Send(string userId, NotificationType notificationType)
        {
            throw new NotImplementedException();
        }

        public Task SendExamStatus(string userId, NotificationType notificationType)
        {
            throw new NotImplementedException();
        }
    }
}

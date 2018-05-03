using NorthOps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NorthOps.Models.Repository
{
    public class UnitOfWork : IDisposable
    {
        private northopsEntities context = new northopsEntities();
        //private GenericRepository<AddressTownCity> addressTownCityRepository;
        //private GenericRepository<AddressStateProvince> addressStateProvinceRepository;


        private GenericRepository<Users> userRepository;
        private GenericRepository<UserRoles> roleRepository;

        public GenericRepository<Users> UserRepository
        {
            get
            {

                if (this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<Users>(context);
                }
                return userRepository;
            }
        }
        public GenericRepository<UserRoles> RoleRepository
        {
            get
            {

                if (this.roleRepository == null)
                {
                    this.roleRepository = new GenericRepository<UserRoles>(context);
                }
                return roleRepository;
            }
        }

        private GenericRepository<JobApplications> jobApplicationRepo;
        public GenericRepository<JobApplications> JobApplicationRepo
        {
            get
            {

                if (this.jobApplicationRepo == null)
                {
                    this.jobApplicationRepo = new GenericRepository<JobApplications>(context);
                }
                return jobApplicationRepo;
            }
        }
        private GenericRepository<AddressTownCities> townCityRepo;
        public GenericRepository<AddressTownCities> TownCityRepo
        {
            get
            {

                if (this.townCityRepo == null)
                {
                    this.townCityRepo = new GenericRepository<AddressTownCities>(context);
                }
                return townCityRepo;
            }
        }
        private GenericRepository<AddressStateProvinces> stateProvinceRepo;
        public GenericRepository<AddressStateProvinces> StateProvinceRepo
        {
            get
            {

                if (this.stateProvinceRepo == null)
                {
                    this.stateProvinceRepo = new GenericRepository<AddressStateProvinces>(context);
                }
                return stateProvinceRepo;
            }
        }
        private GenericRepository<Categories> categoryRepo;
        public GenericRepository<Categories> CategoryRepo
        {
            get
            {

                if (this.categoryRepo == null)
                {
                    this.categoryRepo = new GenericRepository<Categories>(context);
                }
                return categoryRepo;
            }
        }

        private GenericRepository<Exams> examRepo;
        public GenericRepository<Exams> ExamRepo
        {
            get
            {

                if (this.examRepo == null)
                {
                    this.examRepo = new GenericRepository<Exams>(context);
                }
                return examRepo;
            }
        }

        private GenericRepository<Questions> questionRepo;
        public GenericRepository<Questions> QuestionRepo
        {
            get
            {

                if (this.questionRepo == null)
                {
                    this.questionRepo = new GenericRepository<Questions>(context);
                }
                return questionRepo;
            }
        }
        private GenericRepository<Choices> choicenRepo;
        public GenericRepository<Choices> ChoiceRepo
        {
            get
            {

                if (this.choicenRepo == null)
                {
                    this.choicenRepo = new GenericRepository<Choices>(context);
                }
                return choicenRepo;
            }
        }

        private GenericRepository<ApplicantAnswers> applicantAnswer;
        public GenericRepository<ApplicantAnswers> ApplicantAnswer
        {
            get
            {
                if (this.applicantAnswer == null)
                {
                    this.applicantAnswer = new GenericRepository<ApplicantAnswers>(context);
                }
                return applicantAnswer;
            }
        }
        private GenericRepository<Applicants> applicant;
        public GenericRepository<Applicants> Applicant
        {
            get
            {
                if (this.applicant == null)
                {
                    this.applicant = new GenericRepository<Applicants>(context);
                }
                return applicant;
            }
        }
        private GenericRepository<PersonalityResults> personalityResult;
        public GenericRepository<PersonalityResults> PersonalityResult
        {
            get
            {
                if (this.personalityResult == null)
                {
                    this.personalityResult = new GenericRepository<PersonalityResults>(context);
                }
                return personalityResult;
            }
        }
        private GenericRepository<TypingSpeeds> typingSpeedRepo;
        public GenericRepository<TypingSpeeds> TypingSpeedRepo
        {
            get
            {
                if (this.typingSpeedRepo == null)
                {
                    this.typingSpeedRepo = new GenericRepository<TypingSpeeds>(context);
                }
                return typingSpeedRepo;
            }
        }
        private GenericRepository<Videos> videoRepo;
        public GenericRepository<Videos> VideoRepo
        {
            get
            {
                if (this.videoRepo == null)
                {
                    this.videoRepo = new GenericRepository<Videos>(context);
                }
                return videoRepo;
            }
        }

        private GenericRepository<EmployeeNotications> employeeNotifcationsRepo;
        public GenericRepository<EmployeeNotications> EmployeeNoticationsRepo
        {
            get
            {
                if (this.employeeNotifcationsRepo == null)
                    this.employeeNotifcationsRepo = new GenericRepository<EmployeeNotications>(context);
                return employeeNotifcationsRepo;
            }
            set => employeeNotifcationsRepo = value;
        }


        private GenericRepository<NotificationsTemplates> notificationTemplatesRepo;
        public GenericRepository<NotificationsTemplates> NotificationTemplatesRepo
        {
            get
            {
                if (this.notificationTemplatesRepo == null)
                    this.notificationTemplatesRepo = new GenericRepository<NotificationsTemplates>(context);
                return notificationTemplatesRepo;
            }
            set { notificationTemplatesRepo = value; }
        }

        //private GenericRepository<FileMange> fileManagementDataRepo;

        //public GenericRepository<FileManagementData> FileManagementDataRepo
        //{
        //    get
        //    {
        //        if (this.fileManagementDataRepo == null)
        //            this.fileManagementDataRepo = new GenericRepository<FileManagementData>(context);
        //        return fileManagementDataRepo;
        //    }
        //    set { fileManagementDataRepo = value; }
        //}


        //private GenericRepository<Campaign> campaignRepo;
        //public GenericRepository<Campaign> CampaignRepo
        //{
        //    get
        //    {
        //        if (this.campaignRepo == null)
        //            this.campaignRepo = new GenericRepository<Campaign>(context);
        //        return campaignRepo;
        //    }
        //    set { campaignRepo = value; }
        //}
        public void Save()
        {
            context.SaveChanges();
        }
        public async Task<int> SaveAsync()
        {
            return await context.SaveChangesAsync();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
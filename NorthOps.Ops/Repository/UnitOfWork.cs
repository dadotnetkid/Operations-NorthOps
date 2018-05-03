using NorthOps.Ops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NorthOps.Ops.Repository
{
    public class UnitOfWork : IDisposable
    {
        private NorthOpsEntities context = new NorthOpsEntities();
        //private GenericRepository<AddressTownCity> addressTownCityRepository;
        //private GenericRepository<AddressStateProvince> addressStateProvinceRepository;


        private GenericRepository<User> userRepository;
        private GenericRepository<UserRole> roleRepository;

        public GenericRepository<User> UserRepository
        {
            get
            {

                if (this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<User>(context);
                }
                return userRepository;
            }
        }
        public GenericRepository<UserRole> RoleRepository
        {
            get
            {

                if (this.roleRepository == null)
                {
                    this.roleRepository = new GenericRepository<UserRole>(context);
                }
                return roleRepository;
            }
        }

        private GenericRepository<JobApplication> jobApplicationRepo;
        public GenericRepository<JobApplication> JobApplicationRepo
        {
            get
            {

                if (this.jobApplicationRepo == null)
                {
                    this.jobApplicationRepo = new GenericRepository<JobApplication>(context);
                }
                return jobApplicationRepo;
            }
        }
        private GenericRepository<AddressTownCity> townCityRepo;
        public GenericRepository<AddressTownCity> TownCityRepo
        {
            get
            {

                if (this.townCityRepo == null)
                {
                    this.townCityRepo = new GenericRepository<AddressTownCity>(context);
                }
                return townCityRepo;
            }
        }
        private GenericRepository<AddressStateProvince> stateProvinceRepo;
        public GenericRepository<AddressStateProvince> StateProvinceRepo
        {
            get
            {

                if (this.stateProvinceRepo == null)
                {
                    this.stateProvinceRepo = new GenericRepository<AddressStateProvince>(context);
                }
                return stateProvinceRepo;
            }
        }
        private GenericRepository<Category> categoryRepo;
        public GenericRepository<Category> CategoryRepo
        {
            get
            {

                if (this.categoryRepo == null)
                {
                    this.categoryRepo = new GenericRepository<Category>(context);
                }
                return categoryRepo;
            }
        }

        private GenericRepository<Exam> examRepo;
        public GenericRepository<Exam> ExamRepo
        {
            get
            {

                if (this.examRepo == null)
                {
                    this.examRepo = new GenericRepository<Exam>(context);
                }
                return examRepo;
            }
        }

        private GenericRepository<Question> questionRepo;
        public GenericRepository<Question> QuestionRepo
        {
            get
            {

                if (this.questionRepo == null)
                {
                    this.questionRepo = new GenericRepository<Question>(context);
                }
                return questionRepo;
            }
        }
        private GenericRepository<Choice> choicenRepo;
        public GenericRepository<Choice> ChoiceRepo
        {
            get
            {

                if (this.choicenRepo == null)
                {
                    this.choicenRepo = new GenericRepository<Choice>(context);
                }
                return choicenRepo;
            }
        }

        private GenericRepository<ApplicantAnswer> applicantAnswer;
        public GenericRepository<ApplicantAnswer> ApplicantAnswer
        {
            get
            {
                if (this.applicantAnswer == null)
                {
                    this.applicantAnswer = new GenericRepository<ApplicantAnswer>(context);
                }
                return applicantAnswer;
            }
        }
        private GenericRepository<Applicant> applicant;
        public GenericRepository<Applicant> Applicant
        {
            get
            {
                if (this.applicant == null)
                {
                    this.applicant = new GenericRepository<Applicant>(context);
                }
                return applicant;
            }
        }
        private GenericRepository<PersonalityResult> personalityResult;
        public GenericRepository<PersonalityResult> PersonalityResult
        {
            get
            {
                if (this.personalityResult == null)
                {
                    this.personalityResult = new GenericRepository<PersonalityResult>(context);
                }
                return personalityResult;
            }
        }
        private GenericRepository<TypingSpeed> typingSpeedRepo;
        public GenericRepository<TypingSpeed> TypingSpeedRepo
        {
            get
            {
                if (this.typingSpeedRepo == null)
                {
                    this.typingSpeedRepo = new GenericRepository<TypingSpeed>(context);
                }
                return typingSpeedRepo;
            }
        }
        private GenericRepository<Video> videoRepo;
        public GenericRepository<Video> VideoRepo
        {
            get
            {
                if (this.videoRepo == null)
                {
                    this.videoRepo = new GenericRepository<Video>(context);
                }
                return videoRepo;
            }
        }


        private GenericRepository<FileManagementData> fileManagementDataRepo;

        public GenericRepository<FileManagementData> FileManagementDataRepo
        {
            get
            {
                if (this.fileManagementDataRepo == null)
                    this.fileManagementDataRepo = new GenericRepository<FileManagementData>(context);
                return fileManagementDataRepo;
            }
            set { fileManagementDataRepo = value; }
        }


        private GenericRepository<Campaign> campaignRepo;
        public GenericRepository<Campaign> CampaignRepo
        {
            get
            {
                if (this.campaignRepo == null)
                    this.campaignRepo = new GenericRepository<Campaign>(context);
                return campaignRepo;
            }
            set { campaignRepo = value; }
        }
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
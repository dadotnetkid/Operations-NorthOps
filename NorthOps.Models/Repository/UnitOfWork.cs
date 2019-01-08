using NorthOps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NorthOps.Models.Repository
{
    public partial class UnitOfWork : IDisposable
    {
        private northopsEntities context = new northopsEntities();
        //private GenericRepository<AddressTownCity> addressTownCityRepository;
        //private GenericRepository<AddressStateProvince> addressStateProvinceRepository;


        private GenericRepository<Recordings> _RecordingsRepo;
        public GenericRepository<Recordings> RecordingsRepo
        {
            get
            {
                if (this._RecordingsRepo == null)
                    this._RecordingsRepo = new GenericRepository<Recordings>(context);
                return _RecordingsRepo;
            }
            set { _RecordingsRepo = value; }
        }

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


        private GenericRepository<Equivalents> _EquivalentsRepo;
        public GenericRepository<Equivalents> EquivalentsRepo
        {
            get
            {
                if (this._EquivalentsRepo == null)
                    this._EquivalentsRepo = new GenericRepository<Equivalents>(context);
                return _EquivalentsRepo;
            }
            set { _EquivalentsRepo = value; }
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


        private GenericRepository<Attendances> _AttendancesRepo;
        public GenericRepository<Attendances> AttendancesRepo
        {
            get
            {
                if (this._AttendancesRepo == null)
                    this._AttendancesRepo = new GenericRepository<Attendances>(context);
                return _AttendancesRepo;
            }
            set { _AttendancesRepo = value; }
        }

        private GenericRepository<Biometrics> _BiometricsRepo;
        public GenericRepository<Biometrics> BiometricsRepo
        {
            get
            {
                if (this._BiometricsRepo == null)
                    this._BiometricsRepo = new GenericRepository<Biometrics>(context);
                return _BiometricsRepo;
            }
            set { _BiometricsRepo = value; }
        }

        private GenericRepository<Campaigns> _CampaignsRepo;
        public GenericRepository<Campaigns> CampaignsRepo
        {
            get
            {
                if (this._CampaignsRepo == null)
                    this._CampaignsRepo = new GenericRepository<Campaigns>(context);
                return _CampaignsRepo;
            }
            set { _CampaignsRepo = value; }
        }

        private GenericRepository<Shifts> _ShiftsRepo;
        public GenericRepository<Shifts> ShiftsRepo
        {
            get
            {
                if (this._ShiftsRepo == null)
                    this._ShiftsRepo = new GenericRepository<Shifts>(context);
                return _ShiftsRepo;
            }
            set { _ShiftsRepo = value; }
        }


        private GenericRepository<RestDays> _RestDaysRepo;
        public GenericRepository<RestDays> RestDaysRepo
        {
            get
            {
                if (this._RestDaysRepo == null)
                    this._RestDaysRepo = new GenericRepository<RestDays>(context);
                return _RestDaysRepo;
            }
            set { _RestDaysRepo = value; }
        }

        private GenericRepository<Schedules> _SchedulesRepo;
        public GenericRepository<Schedules> SchedulesRepo
        {
            get
            {
                if (this._SchedulesRepo == null)
                    this._SchedulesRepo = new GenericRepository<Schedules>(context);
                return _SchedulesRepo;
            }
            set { _SchedulesRepo = value; }
        }


        private GenericRepository<DailyTimeRecords> _DailyTimeRecordsRepo;
        public GenericRepository<DailyTimeRecords> DailyTimeRecordsRepo
        {
            get
            {
                if (this._DailyTimeRecordsRepo == null)
                    this._DailyTimeRecordsRepo = new GenericRepository<DailyTimeRecords>(context);
                return _DailyTimeRecordsRepo;
            }
            set { _DailyTimeRecordsRepo = value; }
        }
        private GenericRepository<UsersInCampaignShift> _UsersInCampaignShiftRepo;
        public GenericRepository<UsersInCampaignShift> UsersInCampaignShiftRepo
        {
            get
            {
                if (this._UsersInCampaignShiftRepo == null)
                    this._UsersInCampaignShiftRepo = new GenericRepository<UsersInCampaignShift>(context);
                return _UsersInCampaignShiftRepo;
            }
            set { _UsersInCampaignShiftRepo = value; }
        }



        private GenericRepository<EducationAttainments> _EducationAttainmentsRepo;
        public GenericRepository<EducationAttainments> EducationAttainmentsRepo
        {
            get
            {
                if (this._EducationAttainmentsRepo == null)
                    this._EducationAttainmentsRepo = new GenericRepository<EducationAttainments>(context);
                return _EducationAttainmentsRepo;
            }
            set { _EducationAttainmentsRepo = value; }
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
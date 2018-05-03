using NorthOps.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NorthOps.Portal.Repository
{
    public class UnitOfWork : IDisposable
    {
        private NorthOpsEntities context = new NorthOpsEntities();
        //private GenericRepository<AddressTownCity> addressTownCityRepository;
        //private GenericRepository<AddressStateProvince> addressStateProvinceRepository;


        private GenericRepository<User> userRepository;
        private GenericRepository<UserRole> roleRepository;
        //private GenericRepository<EmployeeDepartment> departmentRepository;
        //private GenericRepository<EmployeePosition> positionRepository;

        //private GenericRepository<ClientType> clientTypeRepository;
        //private GenericRepository<ClientClassification> clientClassificationRepository;
        //private GenericRepository<ClientCategory> clientCategoryRepository;
        //private GenericRepository<ClientTerritory> clientTerritoryRepository;

        //private GenericRepository<Client> clientRepository;
        //private GenericRepository<ClientContact> clientContactRepository;

        //private GenericRepository<Coverage> coverageRepository;
        //private GenericRepository<CoverageStatus> coverageStatusRepository;
        //private GenericRepository<Appointment> appointmentRepository;



        //public GenericRepository<AddressTownCity> AddressTownCityRepository {
        //    get {

        //        if (this.addressTownCityRepository == null) {
        //            this.addressTownCityRepository = new GenericRepository<AddressTownCity>(context);
        //        }
        //        return addressTownCityRepository;
        //    }
        //}
        //public GenericRepository<AddressStateProvince> AddressStateProvinceRepository {
        //    get {

        //        if (this.addressStateProvinceRepository == null) {
        //            this.addressStateProvinceRepository = new GenericRepository<AddressStateProvince>(context);
        //        }
        //        return addressStateProvinceRepository;
        //    }
        //}



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


        //public GenericRepository<EmployeeDepartment> DepartmentRepository {
        //    get {

        //        if (this.departmentRepository == null) {
        //            this.departmentRepository = new GenericRepository<EmployeeDepartment>(context);
        //        }
        //        return departmentRepository;
        //    }
        //}
        //public GenericRepository<EmployeePosition> PositionRepository {
        //    get {

        //        if (this.positionRepository == null) {
        //            this.positionRepository = new GenericRepository<EmployeePosition>(context);
        //        }
        //        return positionRepository;
        //    }
        //}

        //public GenericRepository<ClientType> ClientTypeRepository {
        //    get {
        //        if (this.clientTypeRepository == null) {
        //            this.clientTypeRepository = new GenericRepository<ClientType>(context);
        //        }
        //        return clientTypeRepository;
        //    }
        //}
        //public GenericRepository<ClientClassification> ClientClassificationRepository {
        //    get {
        //        if (this.clientClassificationRepository == null) {
        //            this.clientClassificationRepository = new GenericRepository<ClientClassification>(context);
        //        }
        //        return clientClassificationRepository;
        //    }
        //}
        //public GenericRepository<ClientCategory> ClientCategoryRepository {
        //    get {
        //        if (this.clientCategoryRepository == null) {
        //            this.clientCategoryRepository = new GenericRepository<ClientCategory>(context);
        //        }
        //        return clientCategoryRepository;
        //    }
        //}
        //public GenericRepository<ClientTerritory> ClientTerritoryRepository {
        //    get {

        //        if (this.clientTerritoryRepository == null) {
        //            this.clientTerritoryRepository = new GenericRepository<ClientTerritory>(context);
        //        }
        //        return clientTerritoryRepository;
        //    }
        //}

        //public GenericRepository<Client> ClientRepository {
        //    get {

        //        if (this.clientRepository == null) {
        //            this.clientRepository = new GenericRepository<Client>(context);
        //        }
        //        return clientRepository;
        //    }
        //}
        //public GenericRepository<ClientContact> ClientContactRepository {
        //    get {

        //        if (this.clientContactRepository == null) {
        //            this.clientContactRepository = new GenericRepository<ClientContact>(context);
        //        }
        //        return clientContactRepository;
        //    }
        //}


        //public GenericRepository<Coverage> CoverageRepository {
        //    get {

        //        if (this.coverageRepository == null) {
        //            this.coverageRepository = new GenericRepository<Coverage>(context);
        //        }
        //        return coverageRepository;
        //    }
        //}
        //public GenericRepository<CoverageStatus> CoverageStatusRepository {
        //    get {

        //        if (this.coverageStatusRepository == null) {
        //            this.coverageStatusRepository = new GenericRepository<CoverageStatus>(context);
        //        }
        //        return coverageStatusRepository;
        //    }
        //}
        //public GenericRepository<Appointment> AppointmentRepository {
        //    get {

        //        if (this.appointmentRepository == null) {
        //            this.appointmentRepository = new GenericRepository<Appointment>(context);
        //        }
        //        return appointmentRepository;
        //    }
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
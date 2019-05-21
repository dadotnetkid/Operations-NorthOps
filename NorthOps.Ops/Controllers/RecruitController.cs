using DevExpress.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models;
using NorthOps.Models.Config;
using NorthOps.Models.Repository;
using NorthOps.Services.EmailService;
using NorthOps.Services.NotificationService.EmailService;
using NorthOps.Services.NotificationService;

namespace NorthOps.Ops.Controllers
{
    //[RoutePrefix("recruit")]
    public class RecruitController : Controller
    {
        #region Identity
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private UnitOfWork unitOfWork = new UnitOfWork();
        #endregion


        public RecruitController() { }
        public RecruitController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ActionResult PcRecruitmentCallbackPartial()
        {
            ViewBag.newId = Guid.NewGuid().ToString();
            return PartialView("_PcRecruitmentCallbackPartial");
        }

        #region Applicants
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult ApplicantGridViewPartial()
        {
            //var model = unitOfWork.UserRepository.Get(includeProperties: "UserRoles");
            var model = unitOfWork.UserRepository.Get(includeProperties: "UserRoles")
                .Where(x => x.UserRoles.FirstOrDefault()?.Name == "Applicant");
            return PartialView("_EditApplicantPartial", model);
            //return PartialView("_ApplicantGridViewPartial", unitOfWork.UserRepository.Get());
        }



        #endregion
        #region Applications
        [Route("recruit/job-applications"), HttpGet]
        public ActionResult JobApplications(bool? isExamPassed, bool? isTraining)
        {
            ViewBag.isExamPassed = isExamPassed;
            ViewBag.isTraining = isTraining;
            return View();
        }

        public ActionResult EditApplicantPartial([ModelBinder(typeof(DevExpressEditorsBinder))]Guid? Id, bool? isExamPassed, bool? isTraining)
        {
            var model = new UnitOfWork().JobApplicationRepo.Find(m => m.JobApplicationId == Id);
            if (isExamPassed == true && isTraining == true)
            {
                ViewBag.Gridname = "JobApplicationGridTraining";
            }
            else if (isExamPassed == true)
            {
                ViewBag.Gridname = "JobApplicationGridShortlist";
            }

            return PartialView("_EditApplicantPartial", model);
        }
        [ValidateInput(false)]
        public ActionResult JobApplicationGridPartial(bool? isExamPassed, bool? isTraining)
        {
            //x.Users.UserRoles.Any(u => u.Name == "Applicant") &&
            var model = unitOfWork.JobApplicationRepo.Get(includeProperties: "Users,Users.UserRoles").Where(x => x.IsExamPassed == isExamPassed && x.Training == isTraining);
            ViewBag.isExamPassed = isExamPassed;
            ViewBag.isTraining = isTraining;
            return PartialView("_JobApplicationGridPartial", model);
        }


        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> JobApplicationGridPartialUpdate(JobApplications item, bool? isExamPassed, bool? isTraining)
        {
            ViewBag.isExamPassed = isExamPassed;
            ViewBag.isTraining = isTraining;
            if (isExamPassed == true && isTraining == true)
            {
                ViewBag.Gridname = "JobApplicationGridTraining";
            }
            else if (isExamPassed == true)
            {
                ViewBag.Gridname = "JobApplicationGridShortlist";
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var job = await unitOfWork.JobApplicationRepo.GetByIDAsync(item.JobApplicationId);

                    if ((item.ContractDate != null && job.ContractDate == null) && job.ContractDate != item.ContractDate)
                        await new EmailServices(new RecruitmentEmailService(UserManager, item)).Send(job.UserId, "NorthOps Contract Signing Date", NotificationType.Contract);
                    //   await UserManager.SendEmailAsync(job.UserId, "NorthOps Contract Signing Date", $"NorthOps Contract Signing Date { item.ContractDate}");
                    else if ((item.OnBoardingDate != null && job.OnBoardingDate == null) && item.OnBoardingDate != job.OnBoardingDate)
                        await new EmailServices(new RecruitmentEmailService(UserManager, item)).Send(job.UserId, "NorthOps On BoardingDate Date", NotificationType.OnBoarding);
                    //await UserManager.SendEmailAsync(job.UserId, "NorthOps On BoardingDate Date", $"NorthOps On Boarding Date { item.OnBoardingDate}");
                    else if ((item.TrainingDate != null && job.TrainingDate == null) && item.TrainingDate != job.TrainingDate)
                        await new EmailServices(new RecruitmentEmailService(UserManager, item)).Send(job.UserId, "NorthOps Training Date", NotificationType.Training);
                    //await UserManager.SendEmailAsync(job.UserId, "NorthOps Training Date", $"NorthOps Training Date { item.TrainingDate}");
                    else if ((item.PersonalInterviewDate != null && job.PersonalInterviewDate == null) && job.PersonalInterviewDate != item.PersonalInterviewDate)
                        await new EmailServices(new RecruitmentEmailService(UserManager, item)).Send(job.UserId, "NorthOps Personal Interview Date", NotificationType.PersonalInterview);
                    //await UserManager.SendEmailAsync(job.UserId, "NorthOps Personal Interview Date", $"NorthOps Personal Interview Date { item.PersonalInterviewDate}");
                    else if ((item.PhoneInterviewDate != null && job.PhoneInterviewDate == null) && job.PhoneInterviewDate != item.PhoneInterviewDate)
                        await new EmailServices(new RecruitmentEmailService(UserManager, item)).Send(job.UserId, "NorthOps Phone Interview Date", NotificationType.PhoneInterview);
                    //await UserManager.SendEmailAsync(job.UserId, "NorthOps Phone Interview Date", $"NorthOps Phone Interview Date { item.PhoneInterviewDate}");

                    job.PhoneInterviewDate = item.PhoneInterviewDate;
                    job.PhoneInterview = item.PhoneInterview;
                    job.PersonalInterviewDate = item.PersonalInterviewDate;
                    job.PersonalInterview = item.PersonalInterview;
                    job.TrainingDate = item.TrainingDate;
                    job.Training = item.Training;
                    job.OnBoardingDate = item.OnBoardingDate;
                    job.OnBoarding = item.OnBoarding;
                    job.Contract = item.Contract;
                    job.ContractDate = item.ContractDate;

                    unitOfWork.JobApplicationRepo.Update(job);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.JobApplicationRepo.Get(includeProperties: "Users,Users.UserRoles").Where(x => x.IsExamPassed == isExamPassed && x.Training == isTraining);
            return PartialView("_JobApplicationGridPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult JobApplicationGridPartialDelete(System.Guid JobApplicationId, bool? isExamPassed, bool? isTraining)
        {
            ViewBag.isExamPassed = isExamPassed;
            ViewBag.isTraining = isTraining;
            if (isExamPassed == true && isTraining == true)
            {
                ViewBag.Gridname = "JobApplicationGridTraining";
            }
            else if (isExamPassed == true)
            {
                ViewBag.Gridname = "JobApplicationGridShortlist";
            }
            if (JobApplicationId != null)
            {
                try
                {
                    // Insert here a code to delete the item from your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_JobApplicationGridPartial", unitOfWork.JobApplicationRepo.Get(includeProperties: "Users").Where(x => x.Users.UserRoles.Any(m => m.Name == "Applicant")));
        }

        #endregion
        #region OpenExamPartial
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> OpenExamPartial(string UserId, bool? Open)
        {
            if (Open == true)
            {
                var exams = unitOfWork.ExamRepo.Get();
                foreach (var i in exams)
                {
                    unitOfWork.Applicant.Insert(new Applicants()
                    {
                        ApplicantId = Guid.NewGuid(),
                        ExamId = i.ExamId,
                        UserId = UserId

                    });
                    unitOfWork.Save();
                }
                await UserManager.SendEmailAsync(UserId, "Your exam is ready", $"Your exam is ready. Please log in to your NorthOps account and click the exam button to start your online exam.<br/> or click on the link below to proceed to your log in page. <br/> <a href='http://portal.northops.asia/applicant-exam/index'>click here</a>");
            }
            return PartialView("_btnOpenExamPartial", unitOfWork.Applicant.Get(filter: m => m.UserId == UserId).FirstOrDefault());
        }
        [HttpPost]
        public async Task<ActionResult> ResendExaminationNotification(string UserId)
        {
            await UserManager.SendEmailAsync(UserId, "Your exam is ready", $"Your exam is ready. Please log in to your NorthOps account and click the exam button to start your online exam.<br/> or click on the link below to proceed to your log in page. <br/> <a href='http://portal.northops.asia/applicant-exam/index'>click here</a>");
            return PartialView("_btnOpenExamPartial", unitOfWork.Applicant.Get(filter: m => m.UserId == UserId).FirstOrDefault());
        }

        #endregion

        #region Notify Status in Exam

        public async Task<ActionResult> NotifyExamResultPartial(string userId, bool? isPassed)
        {
            try
            {
                if (isPassed != null)
                {
                    var applicant = unitOfWork.JobApplicationRepo.Find(m => m.UserId == userId);
                    applicant.IsExamPassed = isPassed;
                    applicant.IsExamPassedDate = DateTime.Now;
                    await unitOfWork.SaveAsync();
                    var email = new EmailServices(new RecruitmentEmailService(UserManager, applicant));
                    if (isPassed == true)
                    {
                        await email.Send(userId, "Northops", NotificationType.IsExamPassed);
                    }
                    else
                    {
                        await email.Send(userId, "Northops", NotificationType.IsExamFailed);
                    }
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }


            var model = unitOfWork.JobApplicationRepo.Find(m => m.UserId == userId);
            return PartialView("_btnNotifyExamResultPartial", model);
        }

        #endregion

        #region NotifyApplicatnt

        public async Task<HttpStatusCodeResult> NotifyApplicantPartial(string userId, bool isPassed)
        {
            try
            {
                var res = unitOfWork.JobApplicationRepo.Find(m => m.UserId == userId);
                res.IsPersonalInterviewPassed = isPassed;
                await new NotificationService(new ApplicantStatusNotificationService()).NotifyPersonalInterviewStatus(userId, isPassed);
                await new EmailServices(new ApplicantStatusEmailService(UserManager, res)).SendApplicantStatus(userId, isPassed);
                unitOfWork.Save();
            }
            catch (Exception e)
            {

            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion

        #region Notify Personal Interview Result

        public async Task<ActionResult> btnNotifyPersonalResultPartial([ModelBinder(typeof(DevExpressEditorsBinder))]string userId, [ModelBinder(typeof(DevExpressEditorsBinder))]bool? isPassed)
        {
            try
            {
                if (isPassed != null)
                {
                    var applicant = unitOfWork.JobApplicationRepo.Find(m => m.UserId == userId);
                    applicant.IsPersonalInterviewPassed = isPassed;
                    await unitOfWork.SaveAsync();
                    var email = new EmailServices(new RecruitmentEmailService(UserManager, applicant));
                    if (isPassed == true)
                    {
                        await email.Send(userId, "NorthOps", NotificationType.IsPhoneInterviewPassed);
                    }
                    else
                    {
                        await email.Send(userId, "NorthOps", NotificationType.IsPhoneInterviewPassed);
                    }
                }

            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            var model = unitOfWork.JobApplicationRepo.Find(m => m.UserId == userId);
            return PartialView("_btnNotifyPersonalResultPartial", model);
        }

        #endregion
        #region Notify Phone Interview Result
        public async Task<ActionResult> btnNotifyPhoneResultPartial([ModelBinder(typeof(DevExpressEditorsBinder))]string userId, [ModelBinder(typeof(DevExpressEditorsBinder))]bool? isPassed)
        {
            try
            {
                if (isPassed != null)
                {
                    var applicant = unitOfWork.JobApplicationRepo.Find(m => m.UserId == userId);
                    applicant.IsPhoneInterviewPassed = isPassed;
                    await unitOfWork.SaveAsync();
                    var email = new EmailServices(new RecruitmentEmailService(UserManager, applicant));
                    if (isPassed == true)
                    {
                        await email.Send(userId, "NorthOps", NotificationType.IsPhoneInterviewPassed);
                    }
                    else
                    {
                        await email.Send(userId, "NorthOps", NotificationType.IsPhoneInterviewPassed);
                    }
                }

            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            var model = unitOfWork.JobApplicationRepo.Find(m => m.UserId == userId);
            return PartialView("_btnNotifyPhoneResultPartial", model);
        }
        #endregion


        #region Resume

        public ActionResult Resume(string Id)
        {
            var res = unitOfWork.UserRepository.Find(m => m.Id == Id);
            return View(res);
        }

        #endregion

        [ValidateInput(false)]
        public ActionResult EducationalAttainmentGridViewPartial([ModelBinder(typeof(DevExpressEditorsBinder))]string UserId)
        {
            var model = unitOfWork.EducationAttainmentsRepo.Get(m => m.UserId == UserId);
            ViewBag.UserId = UserId;
            return PartialView("_EducationalAttainmentGridViewPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult EmploymentHistoryGridViewPartial([ModelBinder(typeof(DevExpressEditorsBinder))] string UserId)
        {
            var model = unitOfWork.EmploymentHistoriesRepo.Get(m => m.UserId == UserId);
            ViewBag.UserId = UserId;
            return PartialView("_EmploymentHistoryGridViewPartial", model);
        }
    }
}
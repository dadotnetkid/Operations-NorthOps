using DevExpress.Web.Mvc;
using NorthOps.Models.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models.Config;

namespace NorthOps.Ops.Controllers
{
    [Authorize]
    public class BulkEmailController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        [Route("send-bulk-email")]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult BulkEmailGridViewPartial()
        {
            var model = unitOfWork.UserRepository.Fetch(includeProperties: "Applicants").Where(m => m.Applicants.Any(x => x.IsTaken != true && x.DateTimeTaken == null && x.Result == null)).ToList();
            return PartialView("_BulkEmailGridViewPartial", model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Send(string email, string subject, string body)
        {

            foreach (var i in email.Split(','))
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(i);
                mailMessage.From = new MailAddress("careers@northops.asia", "Northops");
                mailMessage.Body = body;
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;

                EmailService emailService = new EmailService();
                try
                {
                    await emailService.SendAsync(mailMessage);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    ViewBag.Error = e.InnerException;
                }
            }


            return PartialView("bulkEmailSendPartial");
        }

        public ActionResult HtmlEditorPartial()
        {
            return PartialView("_HtmlEditorPartial");
        }
    }


}
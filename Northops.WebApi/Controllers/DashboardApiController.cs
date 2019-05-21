using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Config;
using NorthOps.Models.Repository;

namespace Northops.WebApi.Controllers
{
    [Authorize]
    public class DashboardApiController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private string UserId => User.Identity.GetUserId();


        [HttpPost]
        [Route("api-dashboard")]
        public IHttpActionResult Post()
        {
            if (unitOfWork.UserRepository.Fetch(m => m.Id == UserId).Select(x => new { x.EmailConfirmed }).FirstOrDefault()?.EmailConfirmed == false)
            {
                return Ok(new
                {
                    Status = "Users need to confirm email address"
                });
            }
            else if (!unitOfWork.Applicant.Fetch(m => m.UserId == UserId).Any())
            {
                return Ok(new
                {
                    Status = "Waiting for your exam to be assigned"
                });
            }
            else if (!unitOfWork.Applicant.Fetch(m => m.UserId == UserId).Any(m => m.IsTaken != null))
            {
                return Ok(new
                {
                    Status = "You can now take your examination"
                });
            }
            else
            {
                return Ok(new
                {
                    Status = "Waiting for the notification"
                });
            }

        }

        [HttpPost]
        [Route("api-send-feedback")]
        public IHttpActionResult SendFeedback()
        {
            try
            {
                var subject = HttpContext.Current.Request.Params["subject"];
                var body = HttpContext.Current.Request.Params["body"];
                var user = unitOfWork.UserRepository.Find(m => m.Id == UserId);
                EmailService emailService = new EmailService();
                var email = new MailMessage()
                {
                    From = new MailAddress(user.Email, user.FullName),
                    Body = body,
                    Subject = subject

                };
                email.To.Add(new MailAddress("support@northops.asia", "Support"));


                emailService.SendAsync(email);
            }
            catch (Exception e)
            {

            }


            return Ok();
        }
    }
}

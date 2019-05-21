using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.OData;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Northops.WebApi.Models;
using NorthOps.Models;
using NorthOps.Models.Config;
using NorthOps.Models.Repository;

namespace Northops.WebApi.Controllers
{
    public class MemberApiController : ApiController
    {

        ApplicationUserManager UserManager => HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        private UnitOfWork unitOfWork = new UnitOfWork();
        String UserId => User.Identity.GetUserId();

        [EnableQuery]
        [Route("api-users")]
        public  IEnumerable<Users> Get()
        {
            return unitOfWork.UserRepository.Fetch().ToList();
        }


        [System.Web.Http.AllowAnonymous, System.Web.Http.HttpPost, System.Web.Http.Route("api-forgot-password")]

        public async Task<IHttpActionResult> ForgotPassword()
        {

            string emailAddress = HttpContext.Current.Request.Params["EmailAddress"];
            var user = await UserManager.FindByEmailAsync(emailAddress);
            if (user == null)
                return Ok("cant find email address");
            var token = await UserManager.GeneratePasswordResetTokenAsync(user?.Id);
            var url = $"https://portal.northops.asia/change-password?email={HttpUtility.UrlEncode(user.Email)}&token={HttpUtility.UrlEncode(token)}";
            //this.Url.Link("Default",new {Action = "", Controller = "Member", Email = user.Email, Token = Token});
            var confirmationlink =
                $"<h3>Forgot Password</h3><br/><a href='{url}'>Click here to change your Password</a>";
            await UserManager.SendEmailAsync(userId: user.Id, subject: "Forgot Password", body: confirmationlink);

            return Ok();
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("register")]
        public async Task<IHttpActionResult> Register(RegisterViewModel item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            try
            {
                Users user = new Users() { Id = Guid.NewGuid().ToString(), UserName = item.Email, Email = item.Email };
                var result = await UserManager.CreateAsync(user, item.Password);


                if (result.Succeeded)
                {
                    //  await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    await UserManager.AddToRoleAsync(user.Id, "Applicant");
                    unitOfWork.JobApplicationRepo.Insert(new JobApplications()
                    {
                        JobApplicationId = Guid.NewGuid(),
                        UserId = user.Id
                    });
                    await unitOfWork.SaveAsync();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();

        }
        [HttpPost]
        [Authorize]
        [Route("profile")]
        public async Task<IHttpActionResult> Profile([FromBody]ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await UserManager.FindByIdAsync(UserId);

                user.FirstName = model.FirstName ?? user.FirstName;
                user.LastName = model.LastName ?? user.LastName;
                user.MiddleName = model.MiddleName ?? user.MiddleName;
                user.Cellular = model.PhoneNumber ?? user.Cellular;
                user.Skills = model.Skills ?? user.Skills;
                user.Email = model.Email ?? user.Email;
                user.UserName = model.Email ?? user.Email;


                if (!user.EmailConfirmed)
                {
                    var token = await UserManager.GenerateEmailConfirmationTokenAsync(UserId);
                    var url = $"https://portal.northops.asia/confirm-email?email={HttpUtility.UrlEncode(user.Email)}&token={HttpUtility.UrlEncode(token)}";
                    user.ConfirmationCode = new Random(1000000).Next(0, 1000000);
                    user.CodeExpireDate = DateTime.Now.AddDays(3);
                    var confirmationlink = $"<h2><strong>Hi this is from Northops,Inc</strong></h2>\r\n<p>We need to verify your account by clicking the url below thank you and Godbless</p>\r\n<p><a href=\"\\&quot;{url}\\&quot;\">Click here to verify</a></p>\r\n<p>or</p>\r\n<p>Code: <strong>{ user.ConfirmationCode}</strong></p>";
                    await UserManager.SendEmailAsync(userId: user.Id, subject: "Email Confirmation", body: confirmationlink);

                }
                await UserManager.UpdateAsync(user);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error");
            }

            return Ok();
        }
        [HttpPost]
        [Route("api-get-profile")]
        public IHttpActionResult GetProfile()
        {
            return Ok(
                unitOfWork.UserRepository.Fetch(m => m.Id == UserId).Select(m =>
                    new
                    {
                        m.Email,
                        m.FirstName,
                        m.MiddleName,
                        m.LastName,
                        m.Cellular,
                        m.Skills,
                        m.EmailConfirmed,
                        isHasExam = m.Applicants.Any(x => x.IsTaken == null)

                    }).FirstOrDefault()
                );
        }

        [HttpPost]
        [Route("api-confirm-email")]
        public IHttpActionResult ConfirmEmail()
        {

            string confirmationCode = HttpContext.Current.Request.Params["confirmationCode"];
            Users users = unitOfWork.UserRepository.Find(m => m.Id == UserId);
            object obj = null;

            if (users.ConfirmationCode == Convert.ToInt32(confirmationCode) && users.CodeExpireDate > DateTime.Now)
            {
                users.EmailConfirmed = true;
                obj = new { Confirm = true };
            }
            else if (users.CodeExpireDate < DateTime.Now)
            {
                obj = new { Error = "Code Expire", Confirm = false };
            }
            else if (users.ConfirmationCode != Convert.ToInt32(confirmationCode))
            {
                obj = new { Error = "Confirmation Code is invalid", Confirm = false };
            }
            unitOfWork.Save();
            return Ok(obj);
        }

        [HttpPost]
        [Route("api-resend-email")]
        public async Task<IHttpActionResult> ResendEmail()
        {


            var user = await UserManager.FindByIdAsync(UserId);

            var token = await UserManager.GenerateEmailConfirmationTokenAsync(UserId);
            var url = $"https://portal.northops.asia/confirm-email?email={HttpUtility.UrlEncode(user.Email)}&token={HttpUtility.UrlEncode(token)}";
            user.ConfirmationCode = Startup.random.Next(0, 100000);
            user.CodeExpireDate = DateTime.Now.AddDays(3);
            var confirmationlink = $"<h2><strong>Hi this is from Northops,Inc</strong></h2>\r\n<p>We need to verify your account by clicking the url below thank you and Godbless</p>\r\n<p><a href=\"\\&quot;{url}\\&quot;\">Click here to verify</a></p>\r\n<p>or</p>\r\n<p>Code: <strong>{ user.ConfirmationCode}</strong></p>";
            await UserManager.SendEmailAsync(userId: user.Id, subject: "Email Confirmation", body: confirmationlink);
            UserManager.Update(user);
            return Ok();
        }

        //[HttpPost]
        //[Route("api-external-register")]
        private async Task<IHttpActionResult> ExternalRegister()
        {
            try
            {
                string loginProvider = HttpContext.Current.Request.Params["loginProvider"];
                string providerKey = HttpContext.Current.Request.Params["providerKey"];
                string email = HttpContext.Current.Request.Params["Email"];
                UserLoginInfo userLoginInfo = new UserLoginInfo(loginProvider, providerKey);
                var user = UserManager.FindByEmail(email) ?? new Users() { Email = email, UserName = email, Id = Guid.NewGuid().ToString(), EmailConfirmed = true };
                if (UserManager.FindByEmail(email) == null)
                {
                    await UserManager.CreateAsync(user);
                }
                await UserManager.AddLoginAsync(user.Id, userLoginInfo);


                return Ok(await generateAccessToken(email, "", user.Id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("api-external-login")]
        public async Task<IHttpActionResult> ExternalLogin()
        {
            string loginProvider = HttpContext.Current.Request.Params["loginProvider"];
            string providerKey = HttpContext.Current.Request.Params["providerKey"];
            string Email = HttpContext.Current.Request.Params["Email"];

            var user = await UserManager.FindAsync(new UserLoginInfo(loginProvider, providerKey));
            if (user == null)
            {
                return await ExternalRegister();
            }
            else
            {
                return Ok(await generateAccessToken(Email, "", user.Id));
            }
        }

        private async Task<object> generateAccessToken(string userName, string fullName, string userId)
        {
            var tokenExpiration = TimeSpan.FromDays(1);
            string loginProvider = HttpContext.Current.Request.Params["loginProvider"];
            ClaimsIdentity identity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);


            // This is very important as it will be used to populate the current user id 
            // that is retrieved with the User.Identity.GetUserId() method inside an API Controller

            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            var props = new AuthenticationProperties(data);

            var user = UserManager.FindById(userId);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id, null, "LOCAL_AUTHORITY"));
            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName, null, loginProvider));


            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromDays(14));
            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            return new
            {
                userName = userName,
                userId = userId,
                fullName = fullName,
                access_token = accessToken,
                expires_in = tokenExpiration.TotalSeconds,
            };
        }
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }
    }
}

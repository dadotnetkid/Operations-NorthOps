using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using NorthOps.AspIdentity;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Portal.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set
            {
                _signInManager = value;
            }
        }
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
        public MemberController() { }
        public MemberController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }


        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var user = UserManager.FindByEmail(model.Email);
            var result = await SignInManager.PasswordSignInAsync(user?.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }


        #region Email Verifications
        [AllowAnonymous, Route("confirm-email")]
        public async Task<ActionResult> ConfirmEmail(string Email, string Token)
        {
            try
            {
                var user = await UserManager.FindByEmailAsync(Email);
                IdentityResult result = await UserManager.ConfirmEmailAsync(user.Id, Token);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, true, true);
                }
                return View("ConfirmEmail", result);
            }
            catch (Exception ex)
            {

                return View("ConfirmEmail", new IdentityResult());



            }

        }
        [AllowAnonymous, Route("resend-email-verification")]
        public async Task<ActionResult> ResendEmailVerification()
        {
            var UserId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(UserId);
            var Token = await UserManager.GenerateEmailConfirmationTokenAsync(UserId);
            var confirmationlink = $"<a href='{Url.Action("ConfirmEmail", "Member", new { Email = user.Email, Token = Token }, Request.Url.Scheme)}'>Click here to verify</a>";
            await UserManager.SendEmailAsync(userId: user.Id, subject: "Email Confirmation", body: confirmationlink);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion


        #region Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new Users { Id = Guid.NewGuid().ToString(), UserName = model.Email, Email = model.Email, CreatedDate = DateTime.Now };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    await UserManager.AddToRoleAsync(user.Id, "Applicant");
                    unitOfWork.JobApplicationRepo.Insert(new JobApplications()
                    {
                        JobApplicationId = Guid.NewGuid(),
                        UserId = user.Id
                    });
                    await unitOfWork.SaveAsync();
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion
        #region Profile

        public new ActionResult Profile(string Id)
        {
            var user = unitOfWork.UserRepository.GetByID(Id ?? User.Identity.GetUserId());

            return View(user);
        }
        [HttpPost]
        public new async Task<ActionResult> Profile(Users model)
        {
            var user = new Users();
            if (ModelState.IsValid)
            {
                user = await UserManager.FindByIdAsync(model.Id);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.MiddleName = model.MiddleName;
                user.Gender = model.Gender;
                user.BirthDate = model.BirthDate;
                user.AddressLine1 = model.AddressLine1;
                user.AddressLine2 = model.AddressLine2;
                user.TownCity = model.TownCity;
                user.Cellular = model.Cellular;
                user.Religion = model.Religion;
                user.Citizenship = model.Citizenship;
                user.Languages = model.Languages;
                user.CivilStatus = model.CivilStatus;
                user.Skills = model.Skills;
                user.Email = model.Email;
                user.UserName = model.Email;
                await UserManager.UpdateAsync(user);
                // unitOfWork.UserRepository.Update(user);
                // await unitOfWork.SaveAsync();
                if (!user.EmailConfirmed)
                {
                    var UserId = User.Identity.GetUserId();
                    var Token = await UserManager.GenerateEmailConfirmationTokenAsync(UserId);
                    var confirmationlink = $"<a href='{Url.Action("ConfirmEmail", "Member", new { Email = model.Email, Token = Token }, Request.Url.Scheme)}'>Click here to verify</a>";
                    await UserManager.SendEmailAsync(userId: user.Id, subject: "Email Confirmation", body: confirmationlink);
                }


            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error");
            }
            return PartialView("_ProfileViewPartial", user);
        }
        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        public ActionResult LoginStatus()
        {
            return PartialView("_LoginStatus", GetLoginStatus());
        }

        private object GetLoginStatus()
        {
            //return unitOfWork.UserRepository.Get().Where(u => u.Id == User.Identity.GetUserId()).Select(x => new User { Photo = x.Photo ?? MissingImage() }).FirstOrDefault();

            //UserManager.GetRoles(User.Identity.GetUserId()).FirstOrDefault();


            return (from u in UserManager.Users.ToList()
                    where u.Id == User.Identity.GetUserId()
                    select new LoginStatusModel
                    {
                        Name = u.FullName ?? User.Identity.GetUserName(),
                        Position = UserManager.GetRoles(User.Identity.GetUserId()).FirstOrDefault(),
                        HireDate = u.HireDate,
                        Photo = u.Photo ?? MissingImage()
                    }).FirstOrDefault();
        }

        private byte[] MissingImage()
        {
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData("http://portal.northops.asia/content/img/user.png");
            return imageBytes;
        }


        #endregion

    }
}
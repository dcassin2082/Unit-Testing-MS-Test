using AutoMapper;
using Avengers.MVC.Contracts;
using Avengers.MVC.Extensions;
using Avengers.MVC.Identity;
using Avengers.MVC.Models;
using Avengers.RepositoryModels.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PSPRS.Avengers.Core;
using PSPRS.Avengers.Core.Contracts;
using PSPRS.Avengers.Core.Contracts.Services;
using PSPRS.Avengers.Core.Services;
using PSPRS.Avengers.DomainModels;
using PSPRS.Avengers.enums;
using PSPRS.Avengers.Repository;
using PSPRS.Avengers.Repository.Contracts;
using PSPRS.Avengers.Repository.Contracts.Identity;
using PSPRS.Avengers.Repository.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Avengers.MVC.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        //ILoginService login;
        //IWebAuthentication webAuthentication;

        private ApplicationSignInManager signInManager;
        private PSPRSApplicationUserManager userManager;

        private IMemberService memberService;
        private IEmailService emailService;
        private IUserProfilesService userProfilesService;
        private IdentityService identityService;

        public AccountController()
        {
            IMemberRepo memberRepo = new MemberRepo(ConnectionString);
            memberService = new MemberService(memberRepo);

            IEmailRepo emailRepo = new EmailRepo(ConnectionString);
            emailService = new EmailService(emailRepo);

            IUserProfilesRepo userProfilesRepo = new UserProfilesRepo(ConnectionString);
            userProfilesService = new UserProfilesService(userProfilesRepo);

            IIdentityUserRepo identityUserRepo = new IdentityUserRepo(ConnectionString);
            IIdentityRoleRepo identityRoleRepo = new IdentityRoleRepo(ConnectionString);
            identityService = new IdentityService(identityRoleRepo, identityUserRepo);

            //this.webAuthentication = new WebAuthentication();

            //ILoginRepo repo = new LoginRepo(base.ConnectionString);
            //this.login = new LoginService(repo);
        }

        public AccountController(
            ILoginService login,
            IWebAuthentication webAuthentication)
        {
            //this.login = login;
            //this.webAuthentication = webAuthentication;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                signInManager = value;
            }
        }

        public PSPRSApplicationUserManager UserManager
        {
            get
            {
                return userManager ?? HttpContext.GetOwinContext().GetUserManager<PSPRSApplicationUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM vm, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            SignInStatus result = SignInStatus.Failure;
            IdentityUser signedUser = UserManager.FindByEmail(vm.EmailAddress);
            if (signedUser != null)
            {
                result = SignInManager.PasswordSignInAsync(signedUser.UserName, vm.Password, false, shouldLockout: false).Result;
            }
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                //Keep the code below for furture development
                //case SignInStatus.LockedOut:
                //    return View("Lockout");
                //case SignInStatus.RequiresVerification:
                //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "User name or password is incorrect.");
                    return View(vm);
            }
        }

        [AllowAnonymous]
        public ActionResult RegisterMember()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterMember(RegisterMemberVM model)
        {
            //New Net.NetworkCredential("webservices@psprs.com", "taliban9")

            // TODO:  Set mail settings in web.config 
            /*   <system.net>
                    <mailSettings>
                      <smtp configSource="MailSettings.config"></smtp>
                    </mailSettings>
                </system.net>
            */

            string tempPassword = UserManager.GenerateTemporaryPassword();
            var user = new IdentityUser { FirstName = model.FirstName, LastName = model.LastName, UserName = string.Join(".", model.FirstName + model.LastName), EmailAddress = model.EmailAddress, PasswordHash = tempPassword, SecurityStamp = Guid.NewGuid().ToString() };

            MemberDM member = memberService.GetByMemberSSN(model.SSN);
            string errorMessage = null;
            if (member != null)
            {
                if (member.BirthDate == model.DateOfBirth)
                {
                    var result = UserManager.CreateAsync(user, user.PasswordHash);
                    user = UserManager.FindByEmailAsync(model.EmailAddress).Result;
                    memberService.InsertUserMember(member, user.Id);
                    string body = "<h2>Welcome " + model.FirstName + "</h2><br /><a href = {0}>Click here to complete your registration</a>";
                    UserManager.AddToRoleAsync(user.Id, RoleTypes.Member.ToString());
                    SendEmail(model.EmailAddress, user, body);
                    return RedirectToAction("CheckEmail");
                }
            }
            errorMessage = "Invalid SSN or Date of Birth.";
            ModelState.AddModelError("Member Not Found", errorMessage);
            return View(model);
        }

        //private void SendEmail(string emailAddress, IdentityUser user, MemberDM member, string emailBody)
        //{
        //    string code = UserManager.GeneratePasswordResetToken(user.Id);
        //    var callbackUrl = Url.Action("ResetPassword", "Account", new { code = code, emailType = EmailType.EmailConfirmation.ToString() }, protocol: Request.Url.Scheme);
        //    string body = emailService.GenerateEmailConfirmationBody(emailBody, callbackUrl);
        //    string from = "****@gmail.com"; // change to webservices@psprs.com
        //    string to = emailAddress;
        //    string subject = EmailType.EmailConfirmation.ToString();
        //    using (MailMessage message = emailService.GenerateMailMessage(from, to, subject, body))
        //    {
        //        SmtpClient client = emailService.GenerateSmtpClient();
        //        emailService.SendEmail(message, emailAddress, client);
        //        emailService.GenerateEmailHistoryEntry(message, emailAddress, client, Convert.ToInt32(EmailType.EmailConfirmation));
        //    }
        //}

        private void SendEmail(string emailAddress, IdentityUser user, string emailBody)
        {
            string code = UserManager.GeneratePasswordResetToken(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { code = code, emailType = EmailType.EmailConfirmation.ToString() }, protocol: Request.Url.Scheme);
            string body = emailService.GenerateEmailConfirmationBody(emailBody, callbackUrl);
            string from = "****@gmail.com"; // change to webservices@psprs.com
            string to = emailAddress;
            string subject = EmailType.EmailConfirmation.ToString();
            using (MailMessage message = emailService.GenerateMailMessage(from, to, subject, body))
            {
                SmtpClient client = emailService.GenerateSmtpClient();
                emailService.SendEmail(message, emailAddress, client);
                emailService.GenerateEmailHistoryEntry(message, emailAddress, client, Convert.ToInt32(EmailType.EmailConfirmation));
            }
        }

        [AllowAnonymous]
        public ActionResult CheckEmail()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVM model)
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                var result = UserManager.FindByIdAsync(userId);
                if (result.Result.Id == userId)
                {
                    IdentityUser user = result.Result;
                    var changeResult = UserManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
                    if (changeResult.Result.Succeeded)
                    {
                        return RedirectToAction("ChangePasswordResult", "Account");
                    }
                    AddErrors(changeResult);
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ChangePasswordResult()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = UserManager.FindByEmail(model.Email);
                if (user == null || !user.EmailConfirmed)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }
                string code = UserManager.GeneratePasswordResetToken(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { code = code, emailType = Convert.ToInt32(EmailType.ForgotPassword) }, protocol: Request.Url.Scheme);
                string from = "***@gmail.com";
                from = "webservices@psprs.com";
                string to = model.Email;
                string subject = EmailType.ForgotPassword.ToString();
                using (MailMessage message = emailService.GenerateMailMessage(from, to, subject, callbackUrl))
                {
                    SmtpClient client = emailService.GenerateSmtpClient();
                    emailService.SendEmail(message, model.Email, client);
                    emailService.GenerateEmailHistoryEntry(message, model.Email, client, Convert.ToInt32(EmailType.ForgotPassword));
                }
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            return View(model);
        }
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            string emailType = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["emailType"];

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            if (emailType == EmailType.EmailConfirmation.ToString())
            {
                user.Result.EmailConfirmed = true;
                UserManager.UpdateAsync(user.Result);
            }
            var result = UserManager.ResetPasswordAsync(user.Result.Id, model.Code, model.Password);
            if (result.Result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            else
            {
                string code = UserManager.GeneratePasswordResetToken(user.Result.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { code = code, emailType = Convert.ToInt32(EmailType.ForgotPassword) }, protocol: Request.Url.Scheme);
                string from = "***@gmail.com";
                from = "webservices@psprs.com";
                string to = model.Email;
                string subject = EmailType.ForgotPassword.ToString();
                using (MailMessage message = emailService.GenerateMailMessage(from, to, subject, callbackUrl))
                {
                    SmtpClient client = emailService.GenerateSmtpClient();
                    emailService.SendEmail(message, model.Email, client);
                    emailService.GenerateEmailHistoryEntry(message, model.Email, client, Convert.ToInt32(EmailType.ForgotPassword));
                }
                ModelState.AddModelError("Invalid Token", "Token expired.  Please check your email to reset your password");
            }


            //AddErrors(result);
            return View();
        }

        private void AddErrors(Task<IdentityResult> result)
        {
            foreach (var error in result.Result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail(int userId, string code)
        {
            if (userId == 0 || code == null)
            {
                return View("Error");
            }
            var result = UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout()
        {
            if (System.Web.HttpContext.Current.User.IsImpersonating())
            {
                await RevertImpersonationAsync();
                return RedirectToAction("ImpersonateUser", "Account");
            }
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return new RedirectResult("/");
        }

        public async Task RevertImpersonationAsync()
        {
            var context = System.Web.HttpContext.Current;

            if (!System.Web.HttpContext.Current.User.IsImpersonating())
            {
                throw new Exception("Unable to remove impersonation because there is no impersonation");
            }


            var originalUsername = System.Web.HttpContext.Current.User.GetOriginalUsername();

            var originalUser = await UserManager.FindByNameAsync(originalUsername);

            var impersonatedIdentity = await UserManager.CreateIdentityAsync(originalUser, DefaultAuthenticationTypes.ApplicationCookie);
            var authenticationManager = context.GetOwinContext().Authentication;

            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, impersonatedIdentity);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #region UserProfiles
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult ImpersonateUser()
        {
            UserProfilesVM model = new UserProfilesVM
            {
                Users = userProfilesService.GetUsers(),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult ImpersonateUser(UserProfilesVM model)
        {
            IdentityUserDM user = Mapper.Map<IdentityUser, IdentityUserDM>(UserManager.FindById(model.UserID));
            IList<string> userRoles = identityService.GetUserRoles(user);
            var context = System.Web.HttpContext.Current;
            var superUser = context.User.Identity.Name;
            var impersonatedUser =  UserManager.FindByIdAsync(model.UserID);
            var impersonatedIdentity =  UserManager.CreateIdentityAsync(impersonatedUser.Result, DefaultAuthenticationTypes.ApplicationCookie);
            impersonatedIdentity.Result.AddClaim(new Claim(RoleTypes.ImpersonatedUser.ToString(), "true"));
            impersonatedIdentity.Result.AddClaim(new Claim(RoleTypes.SuperAdmin.ToString(), superUser));
            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, impersonatedIdentity.Result);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult UserProfile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult UserProfile(UserProfilesVM model)
        {

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles ="SuperAdmin, Admin")]
        public ActionResult CreateAdmin()
        {
            UserProfilesVM model = new UserProfilesVM
            {
                Users = userProfilesService.GetUsers(),
                Employers = userProfilesService.GetEmployers()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult CreateAdmin(UserProfilesVM model)
        {
            if (ModelState.IsValid)
            {
                List<UserEmployerDM> employers = userProfilesService.GetEmployersByUserID(model.UserID);
                IdentityUserDM user = identityService.GetUserById(model.UserID);
                List<int> employerIds = employers.Select(e => e.ID).ToList();
                foreach (var item in model.Employers)
                {
                    if (item.Checked)
                    {
                        if (employers.Count == 0)
                        {
                            userProfilesService.AddUserToEmployer(model.UserID, item.ID);
                        }
                        else
                        {
                            if (!employerIds.Contains(item.ID))
                                userProfilesService.AddUserToEmployer(model.UserID, item.ID);
                        }
                        IList<string> userRoles = identityService.GetUserRoles(user);
                        if (!userRoles.Contains("Admin"))
                        {
                            identityService.AddToRole(user, "Admin");
                        }
                    }
                    else
                    {
                        if (employers.Count > 0)
                        {
                            if (employerIds.Contains(item.ID))
                                userProfilesService.RemoveUserFromEmployer(model.UserID, item.ID);
                        }
                    }
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            return RedirectToAction("CreateAdmin");
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult CreateUser()
        {
            IdentityUserDM user = identityService.GetUserByName(User.Identity.Name);
            if (user != null)
            {
                List<UserEmployerDM> employers = userProfilesService.GetEmployersByUserID(user.Id);
                if (identityService.IsUserInRole(user, RoleTypes.Admin.ToString()))
                {
                    CreateUserVM model = new CreateUserVM
                    {
                        Employers = userProfilesService.GetEmployersByUserID(user.Id),
                        Roles = identityService.GetRoles().Where(r => r.Name != RoleTypes.SuperAdmin.ToString()).ToList()
                    };
                    return View(model);
                }
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult CreateUser(CreateUserVM model)
        {
            if (ModelState.IsValid)
            {
                string tempPassword = UserManager.GenerateTemporaryPassword();
                var user = new IdentityUser { FirstName = model.FirstName, LastName = model.LastName, UserName = model.Username, EmailAddress = model.EmailAddress, PasswordHash = tempPassword, SecurityStamp = Guid.NewGuid().ToString() };
                var result = UserManager.CreateAsync(user, user.PasswordHash);
                user = UserManager.FindByEmailAsync(model.EmailAddress).Result;
                string body = "<h2>Welcome " + model.Username + "</h2><br /><a href = {0}>Click here to complete your registration</a>";
                SendEmail(model.EmailAddress, user, body);
                List<int> employerIds = model.Employers.Select(e => e.ID).ToList();
                foreach (var employer in model.Employers)
                {
                    if (employer.Checked)
                        userProfilesService.AddUserToEmployer(user.Id, employer.ID);
                }
                foreach (var role in model.Roles)
                {
                    if (role.Checked)
                    {
                        IdentityUserDM dm = Mapper.Map<IdentityUser, IdentityUserDM>(user);
                        identityService.AddToRole(dm, role.Name);
                    }
                }
            }

            return RedirectToAction("CreateUser");
        }

        public JsonResult GetEmployersByUserID(int userID)
        {
            List<UserEmployerDM> employers = userProfilesService.GetEmployersByUserID(userID);
            return Json(employers, JsonRequestBehavior.AllowGet);
            //return Json(userProfilesService.GetEmployersByUserID(userID), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}

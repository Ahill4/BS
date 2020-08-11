/*******************************************************************************
 * @file
 * @brief Manages user interactions with account functions.
 *
 * *****************************************************************************
 *   Copyright (c) 2020 Koninklijke Philips N.V.
 *   All rights are reserved. Reproduction in whole or in part is
 *   prohibited without the prior written consent of the copyright holder.
 *******************************************************************************/

using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BakerySquared.Models;

namespace BakerySquared.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAccountRepository _repository;

        public AccountController()
        {
            this._repository = new EFAccountRepository();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
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

        /// <summary>
        /// Gets user login page.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns> Views/Account/Login.cshtml </returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Verifies user login credentials via AspNetUsers table of DefaultConnection database.
        /// </summary>
        /// <param name="model"> LoginViewModel data properties </param>
        /// <param name="returnUrl"></param>
        /// <returns> 
        /// if(login success): Views/Home/Index.cshtml 
        /// if(login failure): either Views/Shared/Error.cshtml or Views/Account/Login.cshtml 
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ActionResult rtnResult;

            if (!ModelState.IsValid)
            {
                rtnResult = View(model);
            }
            else
            {

                // Require the user to have a confirmed email before they can log on.
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                    {
                        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id);

                        ViewBag.errorMessage = "Email has not been confirmed.  A new confirmation email has been sent to you.";
                        rtnResult = View("Error");
                    }
                    else
                    {
                        // This doesn't count login failures towards account lockout
                        // To enable password failures to trigger account lockout, change to shouldLockout: true
                        var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                        switch (result)
                        {
                            case SignInStatus.Success:
                                {
                                    rtnResult = RedirectToLocal(returnUrl);
                                    break;
                                }
                            case SignInStatus.LockedOut:
                                {
                                    rtnResult = View("Lockout");
                                    break;
                                }
                            case SignInStatus.RequiresVerification:
                                {
                                    rtnResult = RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                                    break;
                                }
                            case SignInStatus.Failure:
                                {
                                    ModelState.AddModelError("", "Invalid login attempt.");
                                    rtnResult = View(model);
                                    break;
                                }
                            default:
                                {
                                    ModelState.AddModelError("", "Invalid login attempt.");
                                    rtnResult = View(model);
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    rtnResult = View(model);
                }               
            }

            return rtnResult;
        }

        /// <summary>
        /// Checks that user has been verified. Used for verification 
        /// of two-factor authentication. Two-factor authentication is 
        /// currently not implemented so this method is not used.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <param name="rememberMe"></param>
        /// <returns>
        /// if(successful): Views/Account/VerifyCode.cshtml
        /// if(failed): Views/Shared/Error.cshtml
        /// </returns>
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            ActionResult rtnResult;

            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                rtnResult = View("Error");
            }
            else
            {
                rtnResult = View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
            }

            return rtnResult;
        }

        /// <summary>
        /// Allows user to verify their account for two-factor aunthentication. 
        /// Two-factor authentication is currently not implemented so this method 
        /// is not used.
        /// </summary>
        /// <param name="model"> VerifyCodeViewModel data properties </param>
        /// <returns>
        /// if(successful): model.ReturnUrl
        /// if(failed): either Views/Shared/Lockout.cshtml or Views/Account/VerifyCody.cshtml 
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            ActionResult rtnResult;

            if (!ModelState.IsValid)
            {
                rtnResult = View(model);
            }
            else
            {
                // The following code protects for brute force attacks against the two factor codes. 
                // If a user enters incorrect codes for a specified amount of time then the user account 
                // will be locked out for a specified amount of time. 
                // You can configure the account lockout settings in IdentityConfig
                var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
                switch (result)
                {
                    case SignInStatus.Success:
                        {
                            rtnResult = RedirectToLocal(model.ReturnUrl);
                            break;
                        }
                    case SignInStatus.LockedOut:
                        {
                            rtnResult = View("Lockout");
                            break;
                        }
                    case SignInStatus.Failure:
                        {
                            rtnResult = View();
                            break;
                        }
                    default:
                        {
                            ModelState.AddModelError("", "Invalid code.");
                            rtnResult = View(model);
                            break;
                        }
                }
            }

            return rtnResult;
        }

        /// <summary>
        /// Gets user registration page.
        /// </summary>
        /// <returns> Views/Account/Register.cshtml </returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Creates new user and sends confirmation email.
        /// </summary>
        /// <param name="model"> RegisterViewModel data properties </param>
        /// <returns>
        /// if(registration successful): Views/Account/ResendConfirmationEmail.cshtml
        /// if(registration failed): either Views/Shared/Error.cshtml or Views/Account/Register.cshtml
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            ActionResult rtnResult;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Account Confirmation", callbackUrl);

                    rtnResult = View("ResendConfirmationEmail");
                }
                else
                {
                    rtnResult = View("Error");
                }
                AddErrors(result);
            }
            else
            {
                rtnResult = View();
            }

            // If we got this far, something failed, redisplay form
            return rtnResult;
        }

        /// <summary>
        /// Gets user resend confirmation email page.
        /// </summary>
        /// <returns> Views/Account/ResendConfirmationEmail.cshtml </returns>
        [AllowAnonymous]
        public ActionResult ResendConfirmationEmail()
        {
            return View();
        }

        /// <summary>
        /// Resends confirmation email to email passed in model.
        /// </summary>
        /// <param name="model"> ResendConfirmationEmailViewModel data propterty (user email address) </param>
        /// <returns>
        /// if(successful): execution resumes in link in user's email to Views/Account/UserSetPassword.cshtml
        /// if(failed): Views/Shared/Error.cshtml
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendConfirmationEmail(ResendConfirmationEmailViewModel model)
        {
            ActionResult rtnResult;

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                    {
                        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id);
                        rtnResult = View();
                    }
                    else
                    {
                        rtnResult = View();
                    }
                }
                else
                {
                    ViewBag.errorMessage = "Incorrect email.";
                    rtnResult = View("Error");
                }
            }
            else
            {
                rtnResult = View("Error");
            }

            // If we got this far, something failed, redisplay form
            return rtnResult;
        }      

        /// <summary>
        /// Gets confirm email page.
        /// </summary>
        /// <param name="userId"> Id of user in AspNetUsers table of DefaultConnection database </param>
        /// <param name="code"> email confirmation token </param>
        /// <returns>
        /// if(successful): Views/Account/ConfirmEmail.cshtml
        /// if(failed): Views/Shared/Error.cshtml
        /// </returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            ActionResult rtnResult;

            if (userId == null || code == null)
            {
                rtnResult = View("Error");
            }
            else
            {
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                rtnResult = View(result.Succeeded ? "ConfirmEmail" : "Error");
            }

            return rtnResult;
        }

        /// <summary>
        /// Gets user set password page.  Execution is sent here from the confirmation email 
        /// for user to choose password.
        /// </summary>
        /// <returns> Views/Account/UserSetPassword.cshtml </returns>
        [AllowAnonymous]
        public ActionResult UserSetPassword()
        {
            return View();
        }

        /// <summary>
        /// Resets password to password chosen by user. 
        /// </summary>
        /// <param name="model"> UserSetPasswordViewModel data properties </param>
        /// <returns>
        /// if(reset successful): Views/Account/Login.cshtml
        /// if(reset failed): either Views/Shared/Error.cshtml or Views/Account/UserSetPassword.cshtml
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserSetPassword(UserSetPasswordViewModel model)
        {
            ActionResult rtnResult;


            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    rtnResult = View("Error");
                }
                else
                {
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    model.Code = code;
                    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                    if (result.Succeeded)
                    {
                        //rtnResult = RedirectToAction("Login", "Account");
                        rtnResult = View("UserSetPasswordConfirmation");
                        AddErrors(result);
                    }
                    else
                    {
                        rtnResult = View();
                    }                   
                }                
            }
            else
            {
                rtnResult = View();
            }

            // If we got this far, something failed, redisplay form
            return rtnResult;
        }

        /// <summary>
        /// Gets user forgot password page.
        /// </summary>
        /// <returns> Views/Account/ForgotPassword.cshtml </returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Sends email for resetting user password to email passed in model.
        /// </summary>
        /// <param name="model"> ForgotPasswordViewModel data property (user email address) </param>
        /// <returns> 
        /// if(successful): Views/Account/ForgotPasswordConfirmation.cshtml then,
        ///                 execution resumes in link in user's email to Views/Account/ResetPassword.cshtml
        /// if(failed): Views/Account/ForgotPassword.cshtml
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ActionResult rtnResult;

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    rtnResult = View("ForgotPasswordConfirmation");
                }
                else
                {
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Password Reset", callbackUrl);

                    rtnResult = RedirectToAction("ForgotPasswordConfirmation", "Account");
                }               
            }
            else
            {
                rtnResult = View("Error");
            }

            // If we got this far, something failed, redisplay form
            return rtnResult;
        }

        /// <summary>
        /// Gets forgot password confirmaiton page.
        /// </summary>
        /// <returns> Views/Account/ForgotPasswordConfirmation.cshtml </returns>
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Gets user reset password page.
        /// </summary>
        /// <param name="code"> password reset token </param>
        /// <returns>
        /// if(code is valid): Views/Account/ResetPassword.cshtml
        /// if(code is invalid): Views/Shared/Error.cshtml
        /// </returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        /// <summary>
        /// Resets user password.
        /// </summary>
        /// <param name="model"> ResetPasswordViewModel data properties </param>
        /// <returns>
        /// if(reset successful): ResetPasswordconfirmation action of AccountController.cs
        /// if(reset failed): Views/Account/ResetPassword.cshtml
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ActionResult rtnResult;

            if (!ModelState.IsValid)
            {
                rtnResult = View(model);
            }
            else
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    rtnResult = RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ResetPasswordConfirmation", "Account");
                    }
                    else
                    {
                        rtnResult = View("Error");
                    }
                    AddErrors(result);
                }                
            }
            
            return rtnResult;
        }

        /// <summary>
        /// Gets reset password confirmation page.  Tells user that the password
        /// has been reset.
        /// </summary>
        /// <returns> Views/Account/ResetPasswordConfirmation.cshtml </returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        /// <summary>
        /// Gets properties to create two-factor authentication code to send user.
        /// Two-factor authentication is currently not implemented so this method is 
        /// not used.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="rememberMe"></param>
        /// <returns>
        /// if(successful): Views/Account/SendCode.cshtml
        /// if(failed): Views/Shared/Error.cshtml
        /// </returns>
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            ActionResult rtnResult;

            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                rtnResult = View("Error");
            }
            else
            {
                var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
                var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();

                rtnResult = View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
            }

            return rtnResult;
        }

        /// <summary>
        /// Sends code for two-factor authentication to user. Two-factor 
        /// authentication is currently not implemented so this method is 
        /// not used.
        /// </summary>
        /// <param name="model"> SendCodeViewModel data properties </param>
        /// <returns>
        /// if(successful): sends execution to VerifyCode action of AccoutController.cs
        /// if(failed): Views/Shared/Error.cshtml
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            ActionResult rtnResult;

            if (!ModelState.IsValid)
            {
                rtnResult = View();
            }
            else
            {
                // Generate the token and send it
                if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
                {
                    rtnResult = View("Error");
                }
                else
                {
                    rtnResult = RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });

                }
            }

            return rtnResult;
        }

        /// <summary>
        /// Helper to ExternalLoginConfirmation() to log in user via external 
        /// login provider. External login is currently not implemented so this 
        /// method is not used.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns>
        /// if(successful): sends user to returnUrl
        /// if(failed): Views/Shared/Lockout.cshtml or 
        ///             Views/Shared/Error.cshtml or
        ///             Views/Account/ExternalLoginConfirmation.cshtml
        ///             sends execution to SendCode action in AccountController.cs             
        /// </returns>
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            ActionResult rtnResult;

            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                rtnResult = RedirectToAction("Login");
            }
            else
            {
                // Sign in the user with this external login provider if the user already has a login
                var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        {
                            rtnResult = RedirectToLocal(returnUrl);
                            break;
                        }
                    case SignInStatus.LockedOut:
                        {
                            rtnResult = View("Lockout");
                            break;
                        }
                    case SignInStatus.RequiresVerification:
                        {
                            rtnResult = RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                            break;
                        }
                    case SignInStatus.Failure:
                        {
                            rtnResult = View("Error");
                            break;
                        }
                    default:
                        {
                            // If the user does not have an account, then prompt the user to create an account
                            ViewBag.ReturnUrl = returnUrl;
                            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                            rtnResult = View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
                            break;
                        }
                }
            }

            return rtnResult;
        }

        /// <summary>
        /// Log in user via external login provider. External login is 
        /// currently not implemented so this method is not used.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns>
        /// if(successful): redirects user to returnUrl
        /// if(failed): Views/Account/ExternalLoginConfirmation.cshtml or
        ///             Views/Account/ExternalLoginFailure.cshtml or
        ///             redirects execution to Index action in ManageController.cs
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            ActionResult rtnResult;

            if (User.Identity.IsAuthenticated)
            {
                rtnResult = RedirectToAction("Index", "Manage");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    // Get the information about the user from the external login provider
                    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        rtnResult = View("ExternalLoginFailure");
                    }
                    else
                    {
                        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                        var result = await UserManager.CreateAsync(user);
                        if (result.Succeeded)
                        {
                            result = await UserManager.AddLoginAsync(user.Id, info.Login);
                            if (result.Succeeded)
                            {
                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                rtnResult = RedirectToLocal(returnUrl);
                            }
                            else
                            {
                                rtnResult = View("Error");
                            }
                        }
                        else
                        {
                            rtnResult = View(model);
                        }
                        AddErrors(result);
                    }
                }
                else
                {
                    ViewBag.ReturnUrl = returnUrl;
                    rtnResult = View(model);
                }                
            }

            return rtnResult;
        }

        /// <summary>
        /// Logs user off.
        /// </summary>
        /// <returns> Index action of HomeController.cs </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Floor1", "Home");
        }

        /// <summary>
        /// Gets ExternalLoginFailure page.
        /// </summary>
        /// <returns> Views/Account/ExternalLoginFailure.cshtml </returns>
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        /// <summary>
        /// Gets registered admins page.  Displays list of all registered admins (users with logins).
        /// </summary>
        /// <returns> Views/Account/RegisteredAdmins.cshtml </returns>
        [AllowAnonymous]
        public ActionResult RegisteredAdmins()
        {
            //var context = ApplicationDbContext.Create();
            //var admins = context.Users.ToList();
            var admins = _repository.ToList();

            return View(admins);
        }

        /// <summary>
        /// Override to delete instances of _userManager and _signInManager that are done 
        /// being used.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

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
            ActionResult rtnResult;

            if (Url.IsLocalUrl(returnUrl))
            {
                rtnResult = Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Floor1", "Home");
            }

            return rtnResult;
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

        /// <summary>
        /// Helper to resend confirmation email to an already registered account.
        /// </summary>
        /// <param name="userID"> Id of user in AspNetUsers table of DefaultConnection database </param>
        /// <returns> Url link to ConfirmEmail action of AccountController.cs </returns>
        private async Task<string> SendEmailConfirmationTokenAsync(string userID)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, "Account Confirmation", callbackUrl);

            return callbackUrl;
        }
        #endregion
    }
}
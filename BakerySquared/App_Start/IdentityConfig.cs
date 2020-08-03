/*******************************************************************************
 * @file
 * @brief Configures identity of app users based on classes defined in AspNet.Identity namespace.
 *
 * *****************************************************************************
 *   Copyright (c) 2020 Koninklijke Philips N.V.
 *   All rights are reserved. Reproduction in whole or in part is
 *   prohibited without the prior written consent of the copyright holder.
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using BakerySquared.Models;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;

namespace BakerySquared
{
    public class EmailService : IIdentityMessageService
    {
        /// <summary>
        /// Sends email via SendMail() method
        /// </summary>
        /// <param name="message"> IdentityMessage object to be configured for email </param>
        /// <returns></returns>
        public Task SendAsync(IdentityMessage message)
        {            
            return Task.Factory.StartNew(() =>
            {
                SendMail(message);
            });
        }

        /// <summary>
        /// Sends email to user via SMTP
        /// </summary>
        /// <param name="message"> IdentityMessage object to be configured for email </param>
        void SendMail(IdentityMessage message)
        {
            var msg = new MailMessage();
            msg.From = new MailAddress("bakerysquared1@gmail.com");
            msg.To.Add(new MailAddress(message.Destination));
            msg.Subject = message.Subject;

            switch (msg.Subject)
            {
                case "Account Confirmation":
                    {
                        msg.Body = " To confirm your new Bakery Squared account: <a href=\"" + message.Body + "\"> Confirm Account </a><br/>\n\n";
                        break;
                    }

                case "Password Reset":
                    {
                        msg.Body = " To reset your password for your Bakery Squared account: <a href=\"" + message.Body + "\"> Reset Password </a><br/>\n\n";
                        break;
                    }
            }

            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(msg.Body, null, MediaTypeNames.Text.Html));

            var smtp = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("bakerysquared1", "123Password");
            smtp.Credentials = credentials;
            smtp.EnableSsl = true;

            smtp.Send(msg);
        }
    }

    /// <summary>
    /// Class reserved for sending SMS messages.  
    /// Currently not implemented.
    /// </summary>
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    /// <summary>
    /// Configure the application user manager used in this application. UserManager 
    /// is defined in ASP.NET Identity and is used by the application.
    /// </summary>
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="store"></param>
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        /// <summary>
        /// Creates new ApplicationUserManager object upon login/registration request.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="context"> DB context for storage of user info </param>
        /// <returns></returns>
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    /// <summary>
    /// Configure the application sign-in manager which is used in this application.
    /// </summary>
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}

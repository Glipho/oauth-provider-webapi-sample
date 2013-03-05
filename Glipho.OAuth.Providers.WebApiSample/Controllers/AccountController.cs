using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Glipho.OAuth.Providers.WebApiSample.Controllers
{
    using System.Data;
    using System.Net;
    using System.Web.Security;
    using Glipho.OAuth.Providers.WebApiSample.Models;

    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>
        /// The consumers database client.
        /// </summary>
        private readonly Database.IConsumers consumers;

        /// <summary>
        /// The issued tokens database client.
        /// </summary>
        private readonly Database.IIssuedTokens issuedTokens;

        /// <summary>
        /// The nonces database client.
        /// </summary>
        private readonly Database.INonces nonces;

        /// <summary>
        /// Initialises a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="consumers">The consumers database client.</param>
        /// <param name="issuedTokens">The issued tokens database client.</param>
        /// <param name="nonces">The nonces database client.</param>
        public AccountController(Database.IConsumers consumers, Database.IIssuedTokens issuedTokens, Database.INonces nonces)
        {
            this.consumers = consumers;
            this.issuedTokens = issuedTokens;
            this.nonces = nonces;
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Authorise(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Authorise(LoginModel model, string returnUrl)
        {
            var users = new Code.Users();
            if (users.Exists(model.UserName))
            {
                // Authorise the request
                var oauthServiceProvider = new OAuthServiceProvider(consumers, issuedTokens, nonces);
                oauthServiceProvider.AuthorizePendingRequestToken(model.UserName.ToLower().Trim());
                this.HttpContext.Response.End();
                return new EmptyResult();
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, "The user name does not exist.");
            return View(model);
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    var users = new Code.Users();
                    users.Create(model.UserName);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError(string.Empty, ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #region Helpers
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}

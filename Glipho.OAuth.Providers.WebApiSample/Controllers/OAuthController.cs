using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Glipho.OAuth.Providers.WebApiSample.Controllers
{
    public class OAuthController : Controller
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
        /// Initialises a new instance of the <see cref="OAuthController"/> class.
        /// </summary>
        /// <param name="consumers">The consumers database client.</param>
        /// <param name="issuedTokens">The issued tokens database client.</param>
        /// <param name="nonces">The nonces database client.</param>
        public OAuthController(Database.IConsumers consumers, Database.IIssuedTokens issuedTokens, Database.INonces nonces)
        {
            this.consumers = consumers;
            this.issuedTokens = issuedTokens;
            this.nonces = nonces;
        }


        public void Authorise()
        {
            var oauthServiceProvider = new OAuthServiceProvider(consumers, issuedTokens, nonces);
            var serviceProvider = oauthServiceProvider.ServiceProvider;
            var requestMessage = serviceProvider.ReadAuthorizationRequest(this.Request);

            if (requestMessage != null)
            {
                // This is a browser opening to allow the user to authorize a request token,
                // so redirect to the authorization page, which will automatically redirect
                // to have the user log in if necessary.
                oauthServiceProvider.PendingAuthorisationRequest = requestMessage;
                this.Response.Redirect("~/Account/Authorise");
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void RequestToken()
        {
            var oauthServiceProvider = new OAuthServiceProvider(consumers, issuedTokens, nonces);
            var serviceProvider = oauthServiceProvider.ServiceProvider;
            var requestMessage = serviceProvider.ReadTokenRequest(this.Request);

            if (requestMessage != null)
            {
                var response = serviceProvider.PrepareUnauthorizedTokenMessage(requestMessage);
                serviceProvider.Channel.Send(response);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void AccessToken()
        {
            var oauthServiceProvider = new OAuthServiceProvider(consumers, issuedTokens, nonces);
            var serviceProvider = oauthServiceProvider.ServiceProvider;
            var requestMessage = serviceProvider.ReadAccessTokenRequest(this.Request);

            if (requestMessage != null)
            {
                var response = serviceProvider.PrepareAccessTokenMessage(requestMessage);
                serviceProvider.Channel.Send(response);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

    }
}

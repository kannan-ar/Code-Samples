using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using AuthorizationServer.Models;
using AuthorizationServer.Services;
using System;
using System.Threading.Tasks;

namespace AuthorizationServer.Providers
{
    public class AuthorizationProvider : OpenIdConnectServerProvider
    {
        public override Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context)
        {
            // Note: the OpenID Connect server middleware supports the authorization code, implicit and hybrid flows
            // but this authorization provider only accepts response_type=code authorization/authentication requests.
            // You may consider relaxing it to support the implicit or hybrid flows. In this case, consider adding
            // checks rejecting implicit/hybrid authorization requests when the client is a confidential application.
            if (!context.Request.IsAuthorizationCodeFlow())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedResponseType,
                    description: "Only the authorization code flow is supported by this authorization server.");

                return Task.FromResult(0);
            }

            // Note: to support custom response modes, the OpenID Connect server middleware doesn't
            // reject unknown modes before the ApplyAuthorizationResponse event is invoked.
            // To ensure invalid modes are rejected early enough, a check is made here.
            if (!string.IsNullOrEmpty(context.Request.ResponseMode) && !context.Request.IsFormPostResponseMode() &&
                                                                       !context.Request.IsFragmentResponseMode() &&
                                                                       !context.Request.IsQueryResponseMode())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: "The specified 'response_mode' is unsupported.");

                return Task.FromResult(0);
            }

            IClientService clientService = (IClientService)context.HttpContext.RequestServices.GetService(typeof(IClientService));

            ClientModel client = clientService.Get(context.Request.ClientId);
            // || !string.Equals(context.Request.ClientSecret, client.ClientSecret, StringComparison.Ordinal)
            // Ensure the client_id parameter corresponds to the Postman client.
            if (client == null)
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "The specified client identifier is invalid.");

                return Task.FromResult(0);
            }

            // Ensure the redirect_uri parameter corresponds to the Postman client.
            if (!string.Equals(context.Request.RedirectUri, client.RedirectUrl, StringComparison.Ordinal))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "The specified 'redirect_uri' is invalid.");

                return Task.FromResult(0);
            }

            context.Validate();

            return Task.FromResult(0);
        }

        public override Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            // Reject the token request if it doesn't specify grant_type=authorization_code,
            // grant_type=password or grant_type=refresh_token.
            if (!context.Request.IsAuthorizationCodeGrantType() &&
                !context.Request.IsPasswordGrantType() &&
                !context.Request.IsRefreshTokenGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: "Only grant_type=authorization_code, grant_type=password or " +
                                 "grant_type=refresh_token are accepted by this server.");

                return Task.FromResult(0);
            }

            // Since there's only one application and since it's a public client
            // (i.e a client that cannot keep its credentials private), call Skip()
            // to inform the server the request should be accepted without
            // enforcing client authentication.
            //context.Skip();

            IClientService clientService = (IClientService)context.HttpContext.RequestServices.GetService(typeof(IClientService));

            ClientModel client = clientService.Get(context.ClientId);

            if (client == null)
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "The specified client identifier is invalid.");
                return Task.FromResult(0);
            }

            if (!string.Equals(context.ClientSecret, client.ClientSecret, StringComparison.Ordinal))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "The specified client credentials are invalid.");
                return Task.FromResult(0);
            }

            context.Validate();

            return Task.FromResult(0);
        }
        /*
        public override Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            if (!context.Request.IsAuthorizationCodeFlow())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedResponseType,
                    description: "Only the authorization code flow is supported by this authorization server.");

                return Task.FromResult(0);
            }

            return Task.FromResult(0);
        }*/
    }
}

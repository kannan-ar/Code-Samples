using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using AuthorizationServer.Helpers;
using AuthorizationServer.Services;

namespace AuthorizationServer.Controllers
{
    public class AuthorizationController : Controller
    {
        public IUserService userService;

        public AuthorizationController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("~/connect/authorize")]
        public IActionResult Authorize()
        {
            // Extract the authorization request from the ASP.NET environment.
            var request = HttpContext.GetOpenIdConnectRequest();

            // Note: in a real world application, you'd probably prefer creating a specific view model.
            return View("Authorize", request);
        }

        //[FormValueRequired("submit.Accept")]
        [HttpPost("~/connect/authorize")]
        [ValidateAntiForgeryToken]
        public IActionResult Accept(string client_id, string username, string pwd)
        {
            
            if(!userService.HasUser(client_id, username, pwd))
            {
                return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
            }

            var request = HttpContext.GetOpenIdConnectRequest();

            // Create a new ClaimsIdentity containing the claims that
            // will be used to create an id_token, a token or a code.
            var identity = new ClaimsIdentity(
                OpenIdConnectServerDefaults.AuthenticationScheme,
                OpenIdConnectConstants.Claims.Name,
                OpenIdConnectConstants.Claims.Role);

            identity.AddClaim(OpenIdConnectConstants.Claims.Subject, Guid.NewGuid().ToString(),
                OpenIdConnectConstants.Destinations.AccessToken,
                OpenIdConnectConstants.Destinations.IdentityToken);

            identity.AddClaim(OpenIdConnectConstants.Claims.Name, "Bob le Bricoleur",
                OpenIdConnectConstants.Destinations.AccessToken,
                OpenIdConnectConstants.Destinations.IdentityToken);

            // Create a new authentication ticket holding the user identity.
            var ticket = new AuthenticationTicket(
                new ClaimsPrincipal(identity),
                new AuthenticationProperties(),
                OpenIdConnectServerDefaults.AuthenticationScheme);

            // Set the list of scopes granted to the client application.
            // Note: this sample always grants the "openid", "email" and "profile" scopes
            // when they are requested by the client application: a real world application
            // would probably display a form allowing to select the scopes to grant.
            ticket.SetScopes(new[] {
                /* openid: */ OpenIdConnectConstants.Scopes.OpenId,
                /* email: */ OpenIdConnectConstants.Scopes.Email,
                /* profile: */ OpenIdConnectConstants.Scopes.Profile,
                /* offline_access: */ OpenIdConnectConstants.Scopes.OfflineAccess
            }.Intersect(request.GetScopes()));

            // Returning a SignInResult will ask ASOS to serialize the specified identity to build appropriate tokens.
            // Note: you should always make sure the identities you return contain ClaimTypes.NameIdentifier claim.
            return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }

        [FormValueRequired("submit.Deny")]
        [HttpPost("~/connect/authorize")]
        [ValidateAntiForgeryToken]
        public IActionResult Deny()
        {
            // Notify ASOS that the authorization grant has been denied by the resource owner.
            // Note: OpenIdConnectServerHandler will automatically take care of redirecting
            // the user agent to the client application using the appropriate response_mode.
            return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
        }
    }
}
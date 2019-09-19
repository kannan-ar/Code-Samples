using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DirectoryServer.Models;
using DirectoryServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryServer.Controllers
{
    public class AccountController : Controller
    {
        private IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        private void IdentitySignin(string userName)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, userName));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity)).Wait();
        }

        public IActionResult Login([FromQuery(Name = "ReturnUrl")]string redirectUrl)
        {
            return View(new LoginModel() { ReturnUrl = redirectUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (accountService.IsAdmin(model.UserName, model.Password))
            {
                IdentitySignin(model.UserName);
                return Redirect(model.ReturnUrl ?? "/");
            }
            else
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }
    }
}
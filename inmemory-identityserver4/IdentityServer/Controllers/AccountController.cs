using IdentityServer.Models;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly TestUserStore _users;

        public AccountController(
            IIdentityServerInteractionService interaction,
            TestUserStore users)
        {
            _interaction = interaction;
            _users = users;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!_users.ValidateCredentials(model.UserName, model.Password))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var user = _users.FindByUsername(model.UserName);

                var props = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    RedirectUri = model.ReturnUrl
                };

                if (model.RememberMe)
                {
                    props.IsPersistent = true;
                };

                var isuser = new IdentityServerUser(user.SubjectId)
                {
                    DisplayName = user.Username
                };

                await HttpContext.SignInAsync(isuser, props);

                if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return Redirect("~/");

                
            }

            return View(model);
        }

        public async Task<IActionResult> Error(string errorId)
        {
            var message = await _interaction.GetErrorContextAsync(errorId);

            return Content(message.Error + Environment.NewLine + message.ErrorDescription);
        }
    }
}

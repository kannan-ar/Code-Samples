using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectoryServer.Models;
using DirectoryServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryServer.Controllers
{
    public class UserController : Controller
    {
        private readonly IClientService clientService;
        private readonly IUserService userService;

        public UserController(IClientService clientService, IUserService userService)
        {
            this.clientService = clientService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            return View("List");
        }

        [Authorize]
        public IActionResult Add()
        {
            return View(new UserModel { Clients = clientService.GetAll()});
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(UserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    userService.Add(model);
                    return RedirectToAction("Index");
                }
                catch (InvalidOperationException ex)
                {
                    return View(new ClientModel { Error = ex.Message });
                }
            }
            else
            {
                return View();
            }
        }
    }
}
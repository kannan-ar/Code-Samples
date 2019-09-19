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
    public class ClientController : Controller
    {
        private readonly IClientService clientService;

        public ClientController(IClientService clientService)
        {
            this.clientService = clientService;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View("List");
        }

        [Authorize]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(ClientModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    clientService.Add(model);
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

        [Authorize]
        public IActionResult Cancel()
        {
            return RedirectToAction("Index");
        }
    }
}
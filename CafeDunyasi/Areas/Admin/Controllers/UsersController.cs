using CafeDunyasi.Data;
using CafeDunyasi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Web;

namespace CafeDunyasi.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("users")]
    [Route("admin/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IHtmlLocalizer<UsersController> _localizer;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;

        public UsersController(IHtmlLocalizer<UsersController> localizer, ApplicationDbContext context, UserManager<Users> userManager)
        {
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
        }

        // GET: UsersController
        [Route("")]
        [Route("index")]
        public ActionResult Index()
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Users";

            return View();
        }

        // GET: UsersController/Details/5
        [Route("details")]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsersController/Create
        [Route("create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Edit/5
        [Route("edit")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit")]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        [Route("delete")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete")]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

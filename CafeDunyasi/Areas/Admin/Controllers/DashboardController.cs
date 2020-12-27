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
    [Route("admin")]
    [Route("admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IHtmlLocalizer<DashboardController> _localizer;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;

        public DashboardController(IHtmlLocalizer<DashboardController> localizer, ApplicationDbContext context, UserManager<Users> userManager)
        {
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Dashboard";

            ViewBag.sumLikes = _context.PostLikes.Count();
            ViewBag.sumPosts = _context.Posts.Count();
            ViewBag.sumUsers = _context.Users.Count();
            ViewBag.sumBusinessAccount = _context.BusinessInfo.Count();

            return View();
        }
    }
}

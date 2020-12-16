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

namespace CafeDunyasi.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHtmlLocalizer<ProfileController> _localizer;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;

        public ProfileController(IHtmlLocalizer<ProfileController> localizer, ApplicationDbContext context, UserManager<Users> userManager)
        {
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            ViewBag.profileImg = _context.BusinessInfo.Single(x => x.UsersID == _userManager.GetUserId(HttpContext.User)).AvatarImg;
            ViewBag.BusinessName = _context.BusinessInfo.Single(x => x.UsersID == _userManager.GetUserId(HttpContext.User)).Name;

            return View(_context.Posts.OrderByDescending(x => x.Date).Take(30).ToList());
        }

        [HttpGet]
        public IActionResult Index(string data)
        {
            if(data == null || data == "")
            {
                ViewBag.error = "1";
                return View();
            }
            else
            {
                try
                {
                    var userData = _context.BusinessInfo.Single(x => x.UsersID == data);
                    ViewBag.profileImg = userData.AvatarImg;
                    ViewBag.BusinessName = userData.Name;
                    ViewBag.menuImage = userData.MenuImg;
                    ViewBag.city = userData.City;

                    return View(_context.Posts.Where(x => x.UserID == data).OrderByDescending(x => x.Date).Take(30).ToList());
                }
                catch
                {
                    ViewBag.error = "1";
                    return View();
                }
            }
        }
    }
}

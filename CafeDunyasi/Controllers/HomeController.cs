using CafeDunyasi.Data;
using CafeDunyasi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHtmlLocalizer<HomeController> _localizer;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;

        public HomeController(ILogger<HomeController> logger, IHtmlLocalizer<HomeController> localizer, ApplicationDbContext context, UserManager<Users> userManager)
        {
            _logger = logger;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.profileImg = _context.BusinessInfo.Single(x => x.UsersID == _userManager.GetUserId(HttpContext.User)).AvatarImg;
            ViewBag.BusinessName = _context.BusinessInfo.Single(x => x.UsersID == _userManager.GetUserId(HttpContext.User)).Name;

            List<string> City = new List<string>();
            var ct = _context.City.ToList();
            foreach (var item in ct)
            {
                City.Add(item.Name);
            }

            ViewBag.Cities = City;

            var likes = _context.PostLikes.Where(x => x.UserID == _userManager.GetUserId(HttpContext.User)).ToList();
            ViewData["likes"] = likes;

            return View(_context.Posts.OrderByDescending(x => x.Date).Take(30).ToList());
        }

        [HttpPost]
        public IActionResult Index(string data)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            var likes = _context.PostLikes.Where(x => x.UserID == userId);
            ViewBag.likes = likes;

            if (data == "all")
            {
                ViewBag.profileImg = _context.BusinessInfo.Single(x => x.UsersID == userId).AvatarImg;
                ViewBag.BusinessName = _context.BusinessInfo.Single(x => x.UsersID == userId).Name;

                List<string> City = new List<string>();
                var ct = _context.City.ToList();
                foreach (var item in ct)
                {
                    City.Add(item.Name);
                }

                ViewBag.Cities = City;

                return View(_context.Posts.OrderByDescending(x => x.Date).Take(30).ToList());
            }else if (data == "liked")
            {
                Posts posts = _context.Posts.Single(x => x.UserID == userId);
                posts.LikeCount++;

                PostLikes postLikes = new PostLikes();
                postLikes.PostID = Convert.ToInt32(data);
                postLikes.UserID = userId;
                _context.PostLikes.Add(postLikes);

                _context.SaveChanges();
                return View(_context.Posts.OrderByDescending(x => x.Date).Take(30).ToList());
            }
            else if (data == "unlike")
            {
                //Posts posts = _context.Posts.Single(x => x.UserID == userId);
                //if(posts.LikeCount > 0)
                //    posts.LikeCount--;

                //_context.PostLikes.Remove(_context.PostLikes.Single(res => res.UserID == userId));

                //_context.SaveChanges();
                return View(_context.Posts.OrderByDescending(x => x.Date).Take(30).ToList());
            }
            else
            {
                ViewBag.profileImg = _context.BusinessInfo.Single(x => x.UsersID == userId).AvatarImg;
                ViewBag.BusinessName = _context.BusinessInfo.Single(x => x.UsersID == userId).Name;

                List<string> City = new List<string>();
                City.Add(data);
                var ct = _context.City.ToList();
                foreach (var item in ct)
                {
                    if (item.Name != data)
                        City.Add(item.Name);
                }

                ViewBag.Cities = City;

                var cityArray = _context.BusinessInfo.Where(x => x.City == data).Select(x => x.UsersID);

                return View(_context.Posts.Where(x => cityArray.Contains(x.UserID)).OrderByDescending(x => x.Date).Take(30).ToList());
            }
        }


            [HttpPost]
        public IActionResult CultureManagement(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

            //return RedirectToAction(nameof(Index));
            return LocalRedirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

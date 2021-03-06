﻿using CafeDunyasi.Data;
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
using System.Web;

namespace CafeDunyasi.Controllers
{
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

        [Authorize]
        public IActionResult Index()
        {
            var businessInfo = _context.BusinessInfo.ToList();
            ViewData["businessInfo"] = businessInfo;

            List<string> City = new List<string>();
            
            City.Add("");
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
        [Authorize]
        public IActionResult Index(string data)
        {
            string userId = _userManager.GetUserId(HttpContext.User);

            if (data == "all")
            {
                var businessInfo = _context.BusinessInfo.ToList();
                ViewData["businessInfo"] = businessInfo;

                List<string> City = new List<string>();
                City.Add("");
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
            else
            {
                var businessInfo = _context.BusinessInfo.ToList();
                ViewData["businessInfo"] = businessInfo;

                List<string> City = new List<string>();
                City.Add(data);
                var ct = _context.City.ToList();
                foreach (var item in ct)
                {
                    if (item.Name != data)
                        City.Add(item.Name);
                }

                ViewBag.Cities = City;

                var likes = _context.PostLikes.Where(x => x.UserID == _userManager.GetUserId(HttpContext.User)).ToList();
                ViewData["likes"] = likes;

                var cityArray = _context.BusinessInfo.Where(x => x.City == data).Select(x => x.UsersID);

                return View(_context.Posts.Where(x => cityArray.Contains(x.UserID)).OrderByDescending(x => x.Date).Take(30).ToList());
            }
        }
        [Authorize]
        public JsonResult Like(string postId)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            bool like = _context.PostLikes.Any(x => x.UserID == userId && x.PostID == Convert.ToInt32(postId));

            if (!like)
            {
                Posts posts = _context.Posts.Single(x => x.Id == Convert.ToInt32(postId));
                posts.LikeCount++;

                PostLikes postLikes = new PostLikes();
                postLikes.PostID = Convert.ToInt32(postId);
                postLikes.UserID = userId;
                _context.PostLikes.Add(postLikes);

                _context.SaveChanges();
            }
            else
            {
                Posts posts = _context.Posts.Single(x => x.Id == Convert.ToInt32(postId));
                if (posts.LikeCount > 0)
                    posts.LikeCount--;

                _context.PostLikes.Remove(_context.PostLikes.Single(res => res.PostID == Convert.ToInt32(postId) && res.UserID == userId));

                _context.SaveChanges();
            }

            Posts postslike = _context.Posts.Single(x => x.Id == Convert.ToInt32(postId));
            int likeCt = postslike.LikeCount;

            return Json(likeCt);
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

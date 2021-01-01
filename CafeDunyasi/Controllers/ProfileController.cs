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

                    var likes = _context.PostLikes.Where(x => x.UserID == _userManager.GetUserId(HttpContext.User)).ToList();
                    ViewData["likes"] = likes;

                    ViewBag.userId = _userManager.GetUserId(HttpContext.User);
                    ViewBag.businessUserId = data;
                    ViewBag.businessId = userData.Id;

                    string follow = "false";
                    if (_userManager.GetUserId(HttpContext.User) != data)
                    {
                        List<FollowingAccounts> fa = _context.FollowingAccounts.Where(x => x.UserID == _userManager.GetUserId(HttpContext.User)).ToList();

                        foreach (var item in fa)
                        {
                            if (item.BusinessID == _context.BusinessInfo.Single(x => x.UsersID == data).Id)
                            {
                                follow = "true";
                            }
                        }
                    }
                    ViewBag.follow = follow;

                    return View(_context.Posts.Where(x => x.UserID == data).OrderByDescending(x => x.Date).Take(30).ToList());
                }
                catch
                {
                    ViewBag.error = "1";
                    return View();
                }
            }
        }

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

        public JsonResult Follow(string userId, string bussinesId)
        {
            FollowingAccounts fa = new FollowingAccounts();
            fa.BusinessID = Convert.ToInt32(bussinesId);
            fa.UserID = userId;

            _context.FollowingAccounts.Add(fa);
            _context.SaveChanges();

            return Json("success");
        }

        public JsonResult Unfollow(string userId, string bussinesId)
        {
            FollowingAccounts fa = _context.FollowingAccounts.Single(x => x.UserID == userId);

            _context.FollowingAccounts.Remove(fa);
            _context.SaveChanges();

            return Json("success");
        }
    }
}

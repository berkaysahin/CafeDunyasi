using CafeDunyasi.Data;
using CafeDunyasi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Controllers
{
    public class PostsOfFollowedCafesController : Controller
    {
        private readonly IHtmlLocalizer<PostsOfFollowedCafesController> _localizer;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;

        public PostsOfFollowedCafesController(IHtmlLocalizer<PostsOfFollowedCafesController> localizer, ApplicationDbContext context, UserManager<Users> userManager)
        {
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
    }
}

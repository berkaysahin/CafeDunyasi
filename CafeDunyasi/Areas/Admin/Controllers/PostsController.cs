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
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace CafeDunyasi.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("posts")]
    [Route("admin/posts")]
    [Authorize(Roles = "Admin")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlLocalizer<PostsController> _localizer;
        private readonly UserManager<Users> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostsController(ApplicationDbContext context, IHtmlLocalizer<PostsController> localizer, UserManager<Users> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Posts
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index(string data = null)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Posts";

            if (data == null)
            {
                ViewData["BusinessAccounts"] = _context.BusinessInfo.ToList();
                return View(await _context.Posts.ToListAsync());
            }

            List<Posts> post = await _context.Posts.Where(x => x.UserID == data).ToListAsync();
            if(post.Count > 0)
                ViewData["BusinessAccounts"] = _context.BusinessInfo.Where(x => x.UsersID == post[0].UserID).ToList();

            return View(post);
        }

        // GET: Admin/Posts/Details/5
        [Route("details")]
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Posts";
            if (id == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (posts == null)
            {
                return NotFound();
            }

            return View(posts);
        }

        private void DeleteFile(string path, string file)
        {
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, path);
            string fileURL = Path.Combine(uploadDir, file);

            if (System.IO.File.Exists(fileURL))
            {
                System.IO.File.Delete(fileURL);
            }
        }

        // GET: Admin/Posts/Delete/5
        [Route("delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Posts";
            if (id == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (posts == null)
            {
                return NotFound();
            }

            return View(posts);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Posts";

            var post = _context.Posts.Single(x => x.Id == id);
            var like = _context.PostLikes.ToList();
            foreach (var item in like)
            {
                if (item.PostID == id)
                {
                    _context.PostLikes.Remove(item);
                }
            }

            DeleteFile("images/BusinessImages/post", post.Image);
            _context.Posts.Remove(post);

            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PostsExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}

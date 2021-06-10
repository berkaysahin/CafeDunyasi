using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeDunyasi.Data;
using CafeDunyasi.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CafeDunyasi.Controllers
{
    [Authorize(Roles ="BusinessAccount")]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<Users> _userManager;

        public PostController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<Users> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: Post
        public async Task<IActionResult> Index()
        {
            var likes = _context.PostLikes.Where(x => x.UserID == _userManager.GetUserId(HttpContext.User)).ToList();
            ViewData["likes"] = likes;
            return View(await _context.Posts.Where(y => y.UserID == _userManager.GetUserId(HttpContext.User)).OrderByDescending(x => x.Date).ToListAsync());
        }

        // GET: Post/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserID,ImageFile,Description,Date")] Posts posts)
        {
            if (ModelState.IsValid)
            {
                string fileName = "";
                string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "images/BusinessImages/post");

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                fileName = Guid.NewGuid().ToString() + "-" + posts.ImageFile.FileName;
                string filePath = Path.Combine(uploadDir, fileName);

                posts.Image = fileName;

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    posts.ImageFile.CopyTo(fileStream);
                }

                posts.Date = DateTime.Now;
                posts.UserID = _userManager.GetUserId(HttpContext.User);

                _context.Add(posts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(posts);
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts.FindAsync(id);
            if (posts == null)
            {
                return NotFound();
            }
            return View(posts);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserID,Image,Description,Date,LikeCount")] Posts posts)
        {
            if (id != posts.Id)
            {
                return NotFound();
            }

            try
            {
                //Posts ps = _context.Posts.Single(x => x.Id == posts.Id);
                //posts.Image = ps.Image;
                //posts.LikeCount = ps.LikeCount;
                //posts.UserID = ps.UserID;
                //posts.Date = ps.Date;

                _context.Update(posts);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostsExists(posts.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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
            string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, path);
            string fileURL = Path.Combine(uploadDir, file);

            if (System.IO.File.Exists(fileURL))
            {
                System.IO.File.Delete(fileURL);
            }
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var posts = await _context.Posts.FindAsync(id);

            DeleteFile("images/BusinessImages/post", posts.Image);

            var like = _context.PostLikes.Where(x => x.PostID == id).ToList();
            foreach (var item in like)
            {
                _context.PostLikes.Remove(item);
            }

            _context.Posts.Remove(posts);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PostsExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
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

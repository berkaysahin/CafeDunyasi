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
    [Route("posts")]
    [Route("admin/posts")]
    [Authorize(Roles = "Admin")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlLocalizer<PostsController> _localizer;
        private readonly UserManager<Users> _userManager;

        public PostsController(ApplicationDbContext context, IHtmlLocalizer<PostsController> localizer, UserManager<Users> userManager)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
        }

        // GET: Admin/Posts
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Posts";

            return View(await _context.Posts.ToListAsync());
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

        // GET: Admin/Posts/Create
        [Route("create")]
        public IActionResult Create()
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Posts";
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<IActionResult> Create([Bind("Id,UserID,Image,Description,Date,LikeCount")] Posts posts)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Posts";
            if (ModelState.IsValid)
            {
                _context.Add(posts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(posts);
        }

        // GET: Admin/Posts/Edit/5
        [Route("edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Posts";
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

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserID,Image,Description,Date,LikeCount")] Posts posts)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Posts";
            if (id != posts.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
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
            //}
            return View(posts);
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
            var posts = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(posts);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostsExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}

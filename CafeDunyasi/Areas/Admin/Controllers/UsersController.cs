using CafeDunyasi.Data;
using CafeDunyasi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;

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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(IHtmlLocalizer<UsersController> localizer, ApplicationDbContext context, UserManager<Users> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: UsersController
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index(string data = null)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Users";

            if (data == null) 
            {
                ViewData["BusinessAccounts"] = _context.BusinessInfo.ToList();
                return View(await _context.Users.ToListAsync());
            }

            ViewData["BusinessAccounts"] = _context.BusinessInfo.Where(x => x.UsersID == data).ToList();
            return View(await _context.Users.Where(x => x.Id == data).ToListAsync());
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Ad")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Soyad")]
            public string Surname { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        // GET: UsersController/Create
        [Route("create")]
        public IActionResult Create()
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Users";

            return View();
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<IActionResult> Create(string returnUrl = null)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Users";
            if (ModelState.IsValid)
            {
                Users user = new Users
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    Surname = Input.Surname
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed, redisplay form
            return View();
            //return RedirectToAction(nameof(Index));
        }

        // GET: UsersController/Edit/5
        [Route("edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Users";

            ViewData["BusinessAccounts"] = _context.BusinessInfo.ToList();

            return View(users);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Surname,Email,Password,AccessFailedCount,BusinessAccount," +
                                                                "ConcurrencyStamp,EmailConfirmed,LockoutEnabled,LockoutEnd,NormalizedEmail," +
                                                                "NormalizedUserName,PasswordHash,PhoneNumber,PhoneNumberConfirmed," +
                                                                "SecurityStamp,TwoFactorEnabled,UserName")] Users user)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Users";

            if (id != user.Id)
            {
                return NotFound();
            }

            if (user.Name.Trim() != "" && user.Surname.Trim() != "" && user.Email.Trim() != "")
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(user.Id))
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
            ViewData["BusinessAccounts"] = _context.BusinessInfo.Where(x => x.UsersID == id).ToList();
            return View(user);
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

        // GET: UsersController/Delete/5
        [Route("delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }

            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "Users";

            return View(users);
        }

        // POST: UsersController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Users _user = _context.Users.Single(res => res.Id == id);

                var follow = _context.FollowingAccounts.Where(x => x.UserID == id).ToList();
                foreach (var item in follow)
                {
                    _context.FollowingAccounts.Remove(item);
                }

                var post = _context.Posts.Where(x => x.UserID == id).ToList();
                var like = _context.PostLikes.ToList();
                foreach (var item in like)
                {
                    foreach (var item2 in post)
                    {
                        if (item2.Id == item.PostID)
                        {
                            _context.PostLikes.Remove(item);
                        }
                    }
                }

                foreach (var item in post)
                {
                    DeleteFile("images/BusinessImages/post", item.Image);
                    _context.Posts.Remove(item);
                }

                BusinessInfo bs = _context.BusinessInfo.Single(x => x.UsersID == id);
                foreach (var item in follow)
                {
                    if (item.BusinessID == bs.Id)
                    {
                        _context.FollowingAccounts.Remove(item);
                    }
                }


                DeleteFile("images/BusinessImages/profile", bs.AvatarImg);
                DeleteFile("images/BusinessImages/menu", bs.MenuImg);

                _context.BusinessInfo.Remove(bs);

                await _userManager.RemoveFromRoleAsync(_user, "BusinessAccount");
                _context.Users.Remove(_user);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
        private bool UsersExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

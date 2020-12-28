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
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace CafeDunyasi.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("businissaccounts")]
    [Route("admin/businissaccounts")]
    [Authorize(Roles = "Admin")]
    public class BusinessAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlLocalizer<BusinessAccountsController> _localizer;
        private readonly UserManager<Users> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BusinessAccountsController(ApplicationDbContext context, IHtmlLocalizer<BusinessAccountsController> localizer, UserManager<Users> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/BusinessAccounts
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "BusinessAccounts";

            return View(await _context.BusinessInfo.ToListAsync());
        }

        // GET: Admin/BusinessAccounts/Details/5
        [Route("details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessInfo = await _context.BusinessInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (businessInfo == null)
            {
                return NotFound();
            }
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "BusinessAccounts";
            return View(businessInfo);
        }

        // GET: Admin/BusinessAccounts/Create
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/BusinessAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UsersID,Name,City,AvatarImg,MenuImg")] BusinessInfo businessInfo)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "BusinessAccounts";
            if (ModelState.IsValid)
            {
                _context.Add(businessInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(businessInfo);
        }

        [TempData]
        public string StatusMessage { get; set; }

        public bool BusinessAccount { get; set; }

        public List<string> City { get; set; }

        public string ProfileImageName { get; set; }
        public string MenuImageName { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [Display(Name = "Bussiness Name")]
            public string BusinessName { get; set; }

            [Required]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [Display(Name = "Profile Image")]
            public IFormFile ProfileImage { get; set; }

            [Required]
            [Display(Name = "Menu Image")]
            public IFormFile MenuImage { get; set; }

            [Display(Name = "Profile Image")]
            public IFormFile ProfileImageUpdate { get; set; }

            [Display(Name = "Menu Image")]
            public IFormFile MenuImageUpdate { get; set; }
        }

        private string UploadFile(IFormFile img, string path)
        {
            string fileName = null;
            if (img != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, path);
                fileName = Guid.NewGuid().ToString() + "-" + img.FileName;
                string filePath = Path.Combine(uploadDir, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    img.CopyTo(fileStream);
                }
            }
            return fileName;
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

        // GET: Admin/BusinessAccounts/Edit/5
        [Route("edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "BusinessAccounts";
            if (id == null)
            {
                return NotFound();
            }

            //var businessInfo = await _context.BusinessInfo.FindAsync(id);
            //if (businessInfo == null)
            //{
            //    return NotFound();
            //}


            Users _user = _context.Users.Single(res => res.Id == _userManager.GetUserId(User));
            BusinessAccount = _user.BusinessAccount;

            try
            {
                BusinessInfo bs = _context.BusinessInfo.Single(res => res.Id == id);
                //City = bs.City;

                if (bs != null)
                {
                    Input = new InputModel
                    {
                        BusinessName = bs.Name,
                        City = bs.City,
                    };
                    ProfileImageName = bs.AvatarImg;
                    MenuImageName = bs.MenuImg;
                    City = new List<string>();
                    var ct = _context.City.ToList();
                    foreach (var item in ct)
                    {
                        City.Add(item.Name);
                    }
                    ViewData["Cities"] = City;
                    ViewBag.profileImg = ProfileImageName;
                    ViewBag.menuImg = MenuImageName;
                    return View(Input);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/BusinessAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UsersID,Name,City,AvatarImg,MenuImg")] BusinessInfo businessInfo)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "BusinessAccounts";
            if (id != businessInfo.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(businessInfo);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!BusinessInfoExists(businessInfo.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}



            BusinessInfo bs = _context.BusinessInfo.Single(res => res.Id == id);

            string profileImage;
            string menuImage;

            try
            {
                if (Input.ProfileImageUpdate != null) { 
                    DeleteFile("images/BusinessImages/profile", bs.AvatarImg);
                    profileImage = UploadFile(Input.ProfileImageUpdate, "images/BusinessImages/profile");
                    bs.AvatarImg = profileImage;
                }
            }
            catch
            {

            }

            try
            {
                if (Input.MenuImageUpdate != null) { 
                    DeleteFile("images/BusinessImages/menu", bs.MenuImg);
                    menuImage = UploadFile(Input.MenuImageUpdate, "images/BusinessImages/menu");
                    bs.MenuImg = menuImage;
                }
            }
            catch
            {

            }

            if (Input.City != bs.City)
                bs.City = Input.City;

            if (Input.BusinessName != bs.Name)
                bs.Name = Input.BusinessName;

            _context.SaveChanges();

            StatusMessage = "İşletme profili güncellendi.";

            //return View();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/BusinessAccounts/Delete/5
        [Route("delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["User"] = _context.Users.Single(x => x.Id == _userManager.GetUserId(HttpContext.User));
            ViewBag.whichPage = "BusinessAccounts";
            if (id == null)
            {
                return NotFound();
            }

            var businessInfo = await _context.BusinessInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (businessInfo == null)
            {
                return NotFound();
            }

            return View(businessInfo);
        }

        // POST: Admin/BusinessAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var businessInfo = await _context.BusinessInfo.FindAsync(id);
            _context.BusinessInfo.Remove(businessInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusinessInfoExists(int id)
        {
            return _context.BusinessInfo.Any(e => e.Id == id);
        }
    }
}

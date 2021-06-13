using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CafeDunyasi.Data;
using CafeDunyasi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CafeDunyasi.Areas.Identity.Pages.Account.Manage
{
    public partial class ActivateBusinessAccountModel : PageModel
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;

        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ActivateBusinessAccountModel(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _context = context;

            _webHostEnvironment = webHostEnvironment;
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

        private async Task LoadAsync(Users user)
        {
            Users _user = _context.Users.Single(res => res.Id == _userManager.GetUserId(User));
            BusinessAccount = _user.BusinessAccount;

            try
            {
                BusinessInfo bs = _context.BusinessInfo.Single(res => res.UsersID == _userManager.GetUserId(User));
                //City = bs.City;

                if(bs != null)
                {
                    Input = new InputModel
                    {
                        BusinessName = bs.Name,
                        City = bs.City,
                    };
                    ProfileImageName = bs.AvatarImg;
                    MenuImageName = bs.MenuImg;
                }
            }catch
            {

            }
            City = new List<string>();
            var ct = _context.City.ToList();
            foreach (var item in ct)
            {
                City.Add(item.Name);
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        private string UploadFile(IFormFile img, string path)
        {
           string fileName = null;
            if(img != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, path);

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

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

            if (!Directory.Exists(uploadDir))
                return;

            string fileURL = Path.Combine(uploadDir, file);

            if (System.IO.File.Exists(fileURL))
            {
                System.IO.File.Delete(fileURL);
            }
        }

        public async Task<IActionResult> OnPostAsync(string button)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            string userId = _userManager.GetUserId(User);

            Users _user = _context.Users.Single(res => res.Id == userId);
            if(_user.BusinessAccount == false && button == "Activate")
            {
                _user.BusinessAccount = true;
                _context.SaveChanges();

                string profileImage = UploadFile(Input.ProfileImage, "images/BusinessImages/profile");
                string menuImage = UploadFile(Input.MenuImage, "images/BusinessImages/menu");

                BusinessInfo bf = new BusinessInfo();
                bf.AvatarImg = profileImage;
                bf.City = Input.City;
                bf.MenuImg = menuImage;
                bf.Name = Input.BusinessName;
                bf.UsersID = userId;

                _context.BusinessInfo.Add(bf);
                _context.SaveChanges();

                await _userManager.AddToRoleAsync(user, "BusinessAccount");

                StatusMessage = "İşletme profili açıldı.";
            }
            else if (_user.BusinessAccount == true && button == "Remove")
            {
                BusinessInfo bs = _context.BusinessInfo.Single(res => res.UsersID == userId);
                _user.BusinessAccount = false;

                var follow = _context.FollowingAccounts.Where(x => x.BusinessID == bs.Id).ToList();
                foreach (var item in follow)
                {
                    _context.FollowingAccounts.Remove(item);
                }

                var post = _context.Posts.Where(x => x.UserID == bs.UsersID).ToList();
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

                _context.BusinessInfo.Remove(_context.BusinessInfo.Single(x => x.UsersID == userId));
                _context.SaveChanges();

                

                DeleteFile("images/BusinessImages/profile", bs.AvatarImg);
                DeleteFile("images/BusinessImages/menu", bs.MenuImg);

                var postList = _context.Posts.ToList();
                foreach (var item in postList)
                {
                    if (item.UserID == userId)
                    {
                        DeleteFile("images/BusinessImages/post", item.Image);

                        _context.Posts.Remove(item);
                    }
                }

                await _userManager.RemoveFromRoleAsync(user, "BusinessAccount");

                StatusMessage = "İşletme profili kapatıldı.";
            }
            else
            {
                BusinessInfo bs = _context.BusinessInfo.Single(res => res.UsersID == userId);

                string profileImage;
                string menuImage;

                try
                {
                    if (Input.ProfileImageUpdate != null)
                    {
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
                    if(Input.MenuImageUpdate != null)
                    {
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
            }

            return RedirectToPage();
        }
    }
}

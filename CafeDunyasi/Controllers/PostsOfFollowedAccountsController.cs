using CafeDunyasi.Data;
using CafeDunyasi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class PostsOfFollowedAccountsController : Controller
    {
        private readonly IHtmlLocalizer<PostsOfFollowedAccountsController> _localizer;
        private readonly ILogger<PostsOfFollowedAccountsController> _logger;

        private readonly ApplicationDbContext _context;

        public PostsOfFollowedAccountsController(ILogger<PostsOfFollowedAccountsController> logger, IHtmlLocalizer<PostsOfFollowedAccountsController> localizer, ApplicationDbContext context)
        {
            _logger = logger;
            _localizer = localizer;

            _context = context;
        }
        public IActionResult Index()
        {
            var s = _context.Posts.ToList();
            return View(s);
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

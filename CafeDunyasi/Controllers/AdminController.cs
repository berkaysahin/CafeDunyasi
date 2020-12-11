using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Controllers
{
    public class AdminController : Controller
    {
        [Authorize (Roles = "Admin")] // [AllowAnonymous] ile herkes ulaşsın diyebiliyoruz
        public IActionResult Index()
        {
            return View();
        }
    }
}

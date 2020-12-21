using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        [Authorize(Roles = "Admin")]
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

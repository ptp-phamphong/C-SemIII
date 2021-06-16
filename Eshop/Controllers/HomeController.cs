using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Eshop.Controllers
{
    public class HomeController : Controller
    {
        
        //private readonly ILogger<HomeController> _logger;
        private readonly EshopContext _context;
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        
        public HomeController (EshopContext context)
        {
            _context = context;
            ViewBag.isLogin = "abc";
        }

        public IActionResult Index()
        {
            ViewBag.CurrentUser = HttpContext.Session.GetString("CurrentUser");
            HttpContext.Session.SetString("PageBeing", "Home");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

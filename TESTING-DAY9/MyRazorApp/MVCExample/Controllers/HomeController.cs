using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCExample.Models;

namespace MVCExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetString("UserName", "Siddhakala");
            HttpContext.Session.SetInt32("UserId", 101);
            return View();
        }

        public IActionResult Privacy()
        {
            string? userName = HttpContext.Session.GetString("UserName");
            int? userId = HttpContext.Session.GetInt32("UserId");

            ViewBag.UserName = userName;
            ViewBag.UserId = userId;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

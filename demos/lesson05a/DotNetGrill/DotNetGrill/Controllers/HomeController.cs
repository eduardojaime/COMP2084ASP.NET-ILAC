using DotNetGrill.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DotNetGrill.Controllers
{
    // Think of controller as a section in your website
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // GET /home/index or / (root, landing page)
        public IActionResult Index()
        {
            return View(); // renders /Views/Home/Index.cshtml
        }
        // GET /home/privacy
        public IActionResult Privacy()
        {
            return View(); // renders /Views/Home/Privacy.cshtml
        }

        // GET /home/aboutus
        public IActionResult AboutUs()
        {
            return View(); // renders /Views/Home/AboutUs.cshtml
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

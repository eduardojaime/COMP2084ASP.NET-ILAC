using DotNetGrillWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DotNetGrillWebUI.Controllers
{
    // Controllers represent sections in your app
    // e.g. https://localhost:1234/HOME
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // Each action result method handles an HTTP request to a specific subsection
        // GET /Home/Index or / or /Home because it's the default method
        public IActionResult Index()
        {
            return View(); // renders /Views/Home/Index.cshtml
        }
        // GET /Home/Privacy
        public IActionResult Privacy()
        {
            return View(); // renders /Views/Home/Privacy.cshtml
        }

        // GET /Home/AboutUs
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

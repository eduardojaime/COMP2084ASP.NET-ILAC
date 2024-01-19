using DotNetGrillWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DotNetGrillWebUI.Controllers
{
    // Controllers correspond to Sections in the website
    // GET /Home or / (root)
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // Action Methods correspond to subsections in the website
        // Index is default method
        // GET /Home/Index or /Home or / (root, since homecontroller is default path) 
        public IActionResult Index()
        {
            return View(); // renders /Views/Home/Index.cshtml
        }
        // GET /Home/Privacy
        public IActionResult Privacy()
        {
            return View(); // renders /Views/Home/Privacy.cshtml
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

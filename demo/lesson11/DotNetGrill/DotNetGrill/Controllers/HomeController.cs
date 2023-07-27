using DotNetGrill.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DotNetGrill.Controllers
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
            return View(); // returns a view inside /Views/Home with the same name as the method
        }

        public IActionResult Privacy()
        {
            return View(); // returns a view inside /Views/Home with the same name as the method
        }

        // 1) add action methods in order to handle another page
        // 2) add a corresponding view file in the folder for this controller
        // /ContactUs
        // action results can be JSON, XML, PLAIN TEXT, BINARY FILES OR... HTML (VIEWS)
        public IActionResult ContactUs()
        {
            // returns a rendered view (HTML)
            return View(); // returns /Views/Home/ContactUs.cshtml
        }

        // Add another action method and view for the About page
        public IActionResult About()
        {
            //ViewData["Content"] = "Hello there!";
            return View(); // returns /Views/Home/About.cshtml
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
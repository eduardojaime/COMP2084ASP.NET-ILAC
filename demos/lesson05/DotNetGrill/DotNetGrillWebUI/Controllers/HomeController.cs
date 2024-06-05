using DotNetGrillWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DotNetGrillWebUI.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Add another action method for AboutUs page
        // IActionResult represents a result of an action method which is an HTTP Response
        // it can contain content in different formats: JSON, HTML, XML, etc.
        public IActionResult AboutUs()
        {
            // returns a ViewResult object that contains the HTML content of a view
            // if no view name is provided, it will look for a view with the same name as the action method
            // e.g. AboutUs.cshtml
            return View(); 
        }

        // Add another action method for ContactUs page
        public IActionResult ContactUs()
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

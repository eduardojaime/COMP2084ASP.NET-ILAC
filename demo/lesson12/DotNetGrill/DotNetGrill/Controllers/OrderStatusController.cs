using Microsoft.AspNetCore.Mvc;

namespace DotNetGrill.Controllers
{
    public class OrderStatusController : Controller
    {
        public IActionResult Index()
        {
            // Controllers created manually don't have Views
            return View();
        }

        // Add new Action Result that returns a JSON response (API endpoint)
        public IActionResult GetStatus() { 
            // return dynamic object in JSON format
            return Json(new { Status = "Shipped" });  
        }
    }
}

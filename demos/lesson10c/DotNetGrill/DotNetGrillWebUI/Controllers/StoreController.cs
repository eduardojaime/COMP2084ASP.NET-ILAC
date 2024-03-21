using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetGrillWebUI.Data;
using DotNetGrillWebUI.Models;

namespace DotNetGrillWebUI.Controllers
{
    public class StoreController : Controller
    {
        // Private field that will hold a reference to the dbcontext object provided via DI
        private readonly ApplicationDbContext _context;
        // Constructor Method
        // ASP.NET MVC initializes the controller automatically when needed
        // and will provide a copy of all registered services via DI
        // if we need to use a specific service we can ask for it in the constructor
        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Store
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Store/Browse/3 (3 is the category id)
        public async Task<IActionResult> Browse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // retrieve a list of products filtered by category id
            // LINQ query which has a similar sintax to SQL queries but for C#
            var products = _context.Products
                .Where(p => p.CategoryId == id)
                .OrderBy(p => p.Name)
                .ToList();

            // two ways to send data back to the view
            // 1) viewbag object
            ViewBag.CategoryName = _context.Categories.Find(id).Name; // find the category name by id
            // 2) as a view model
            return View(products);
        }

        // POST: Store/AddToCart
        // values expected: ProductId, Quantity
        [HttpPost]
        public IActionResult AddToCart([FromForm] int ProductId, [FromForm] int Quantity) {
            // get or generate a customer id
            var customerId = GetCustomerId();
            // query the db to get current product price
            var price = _context.Products.Find(ProductId).Price;
            // create and save a new cart object
            var cart = new Cart
            {
                CustomerId = customerId,
                ProductId = ProductId,
                Quantity = Quantity,
                DateCreated = DateTime.UtcNow, // always use UTC time to store datetimes in your dbs
                Price = price
            };
            _context.Carts.Add(cart);
            _context.SaveChanges();
            // redirect to a "Cart" view
            return Redirect("Cart"); // name of action method to trigger next
        }

        // GET: Store/Cart
        public IActionResult Cart() { 
            // retrieve customer id
            var customerId = GetCustomerId();
            // retrieve carts associated to them
            var carts = _context.Carts
                .Where(c => c.CustomerId == customerId)
                .OrderByDescending(c => c.DateCreated)
                .ToList();
            // return a view with the list of carts
            return View(carts);
        }

        private string GetCustomerId()
        {
            // set variable to store customerId
            var customerId = string.Empty;
            // check the session object for a value
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("CustomerId")))
            {
                // if there's no value then use either email address (authenticated users)
                if (User.Identity.IsAuthenticated)
                {
                    customerId = User.Identity.Name;
                }
                else
                {
                    // or a random GUID (anonymous users)
                    customerId = Guid.NewGuid().ToString();
                }
                // save the value in the session object
                HttpContext.Session.SetString("CustomerId", customerId);
            }
            else
            {
                // return the value
                customerId = HttpContext.Session.GetString("CustomerId");
            }
            // return the value
            return customerId;
        }
    }
}

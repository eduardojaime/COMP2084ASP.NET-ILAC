using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetGrill.Data;
using DotNetGrill.Models;

namespace DotNetGrill.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Store
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET Store/Browse/ID
        public IActionResult Browse(int id) {
            // Filter products by id and return
            var products = _context.Products
                .Where(p => p.CategoryId == id) // filter by category id
                .OrderBy(p => p.Name) // sort by name
                .ToList(); // converts result to a list of products
            // two ways of sending data back to the view
            // 1) viewbag object
            ViewBag.CategoryName = _context.Categories.Find(id).Name; // finds a category by id and gets its name
            // 2) as a view model
            return View(products);
        }

        // GET /Store/AddToCart
        // data will come from the form (two input fields: quantity and productid)
        // use modelbinder to link data sent from view to a parameter name in my action method
        public IActionResult AddToCart([FromForm] int ProductId, [FromForm] int Quantity) {
            // retrieve customer ID of current user
            var customerId = GetCustomerId();
            // query the db and get product price at this moment
            var price = _context.Products.Find(ProductId).Price;
            // create and save the cart object
            var cart = new Cart()
            {
                ProductId = ProductId,
                Quantity = Quantity,
                Price = price,
                DateCreated = DateTime.UtcNow,
                CustomerId = customerId
            };
            // save changes to db
            _context.Carts.Add(cart);
            _context.SaveChanges();
            // redirect to cart view
            return Redirect("Cart");
        }

        // GET Store/Cart
        public IActionResult Cart() {
            // show all products that this customer has added to cart
            string customerId = GetCustomerId();
            var carts = _context.Carts
                .Where(c => c.CustomerId == customerId)
                .OrderByDescending(c => c.DateCreated)
                .ToList();
            return View(carts);
        }
        /// <summary>
        /// This method returns the customer id in the session object
        /// Users can be anonymous (unauthenticated) or authenticated
        /// If user is authenticated then customerId is their email address
        /// Otherwise customerId is a random guid
        /// </summary>
        /// <returns>String value representing customerid</returns>
        private string GetCustomerId()
        {
            // initialzie customerid
            string customerId = string.Empty;
            // retrieve customer id from session object, if nothing exists then create one
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("CustomerId"))) {
                if (User.Identity.IsAuthenticated) {
                    customerId = User.Identity.Name; // set to email address
                }
                else
                {
                     customerId = Guid.NewGuid().ToString(); // random guid value for public users
                }
                HttpContext.Session.SetString("CustomerId", customerId);
            }
            // just return anything that's currently stored here
            return HttpContext.Session.GetString("CustomerId");
        }
    }
}

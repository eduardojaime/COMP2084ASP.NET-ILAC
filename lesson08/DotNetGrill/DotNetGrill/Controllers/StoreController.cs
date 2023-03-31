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
              return _context.Categories != null ? 
                          View(await _context.Categories.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }

        // GET handler for /Store/Browse/{id}
        // This action method will show a filtered list of products by category
        public IActionResult Browse(int id)
        {
            // retrieve a list of products by category
            // use dbcontext to get a list of products then filter it using LINQ
            var products = _context.Products
                            .Where(p => p.CategoryId == id)
                            .OrderBy(p => p.Name)
                            .ToList();

            // send data to the view in two ways
            // data in the dynamic viewbag object
            ViewBag.categoryName = _context.Categories.Find(id).Name; // find method retrieves a single element by id
            //     Alternative to  _context.Categories.Where(c=>c.CategoryId == id).FirstOrDefault().Name;
            // data as model
            return View(products);
        }

        // GET handler for /Store/AddToCart
        // data will come from the form elements: two input fields
        // model binder is a background process that links data sent from the request to parameters in my action method
        public IActionResult AddToCart([FromForm] int ProductId, [FromForm] int Quantity)
        {
            // get or generate a customer id > who buys?
            var customerId = GetCustomerId();
            // query the db to get price > how much they pay?
            var price = _context.Products.Find(ProductId).Price;
            // create and save cart object
            var cart = new Cart()
            {
                ProductId = ProductId,
                Quantity = Quantity,
                Price = price,
                DateCreated = DateTime.UtcNow, // best practice, always store datetimes in UTC time
                CustomerId = customerId
            };
            // save to changes database
            _context.Carts.Add(cart);
            _context.SaveChanges();
            // redirect to cart view
            return Redirect("Cart");
        }

        // GET handler for /Store/Cart
        public IActionResult Cart()
        {
            string customerId = GetCustomerId();
            // return a list of elements in the carts table by customerId
            var carts = _context.Carts
                        .Where(c => c.CustomerId == customerId)
                        .OrderByDescending(c => c.DateCreated)
                        .ToList();
            return View(carts);
        }

        /// <summary>
        /// This method will use the session object to store a value to identify the user visiting the site
        /// Users can be anonymous or authenticated
        /// Anonymous users will be identified by a randomly generated GUID
        /// Authenticated users will be identified by their email address
        /// </summary>
        /// <returns>A string value representing a user ID</returns>
        private string GetCustomerId()
        {
            // variable to store generated/retrieved ID value
            string customerId = string.Empty;

            // check the object for a customer id
            // Microsoft.AspNetCore.Http to access GetString
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("CustomerId")))
            {

                if (User.Identity.IsAuthenticated)
                {
                    customerId = User.Identity.Name;
                }
                else
                {
                    customerId = Guid.NewGuid().ToString();
                }

                HttpContext.Session.SetString("CustomerId", customerId);
            }

            return HttpContext.Session.GetString("CustomerId");
        }
    }
}

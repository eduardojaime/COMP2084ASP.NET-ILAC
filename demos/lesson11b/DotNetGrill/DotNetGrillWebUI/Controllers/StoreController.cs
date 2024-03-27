using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetGrillWebUI.Data;
using DotNetGrillWebUI.Models;
using Microsoft.AspNetCore.Authorization;
using DotNetGrillWebUI.Extensions;

namespace DotNetGrillWebUI.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;
        // Use the configuration object to access values from appsettings.json
        private readonly IConfiguration _configuration;
        // Use DI to request the ApplicationDbContext and IConfiguration objects on controller creation
        public StoreController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
            var carts = _context.Carts  //                  SELECT * FROM Carts c 
                 // include the product object to avoid lazy loading
                .Include(c => c.Product) //                 JOIN Products p ...
                .Where(c => c.CustomerId == customerId) //  WHERE c.CustomerId = 'customerId'
                .OrderByDescending(c => c.DateCreated) //   ORDER BY c.DateCreated DESC
                .ToList();
            // LINQ to calculate total price
            var totalAmount = carts.Sum(c => (c.Price * c.Quantity)).ToString("C");
            // send the total amount to the view
            ViewBag.TotalAmount = totalAmount;
            // return a view with the list of carts
            return View(carts);
        }

        // GET: Store/RemoveFromCart/5 (5 is the cart id)
        public IActionResult RemoveFromCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // retrieve the cart object by id
            var cart = _context.Carts.Find(id);
            // remove it from the db
            _context.Carts.Remove(cart);
            _context.SaveChanges();
            // redirect to the Cart action method
            return RedirectToAction("Cart");
        }

        // GET: Store/Checkout
        // It should only be accessible to authenticated users
        [Authorize]
        public IActionResult Checkout() { 
            return View();
        }

        // POST: Store/Checkout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken] // prevent CSRF attacks where someone else tries to submit a form on behalf of the user
        public IActionResult Checkout([FromForm] Order order)
        {
            // populate the 3 special order properties: DateCreated, CustomerId, Total
            order.DateCreated = DateTime.UtcNow; // always use UTC time to store datetimes in your dbs
            order.CustomerId = GetCustomerId();
            order.Total = _context.Carts
                .Where(c => c.CustomerId == order.CustomerId)
                .Sum(c => (c.Price * c.Quantity));
            // all other properties will be populated from the form

            // Store order object in session, so I can retrieve it AFTER payment is successfull
            // Once this happens, I'll retrieve the order, save it to the db and remove the carts
            HttpContext.Session.SetObject("Order", order);
            
            // redirect to the Payment action method
            return RedirectToAction("Payment");
        }

        // GET: Store/Payment
        public IActionResult Payment()
        {
            // retrieve the order object from the session
            var order = HttpContext.Session.GetObject<Order>("Order"); 
            // pass total to view in the viewbag object to display amount
            ViewBag.TotalAmount = order.Total.ToString("C"); // format decimal as string currency $xx.xx
            // TODO: implement payment gateway integration
            // Add publishable key to the viewbag object
            ViewBag.PublishableKey = _configuration["Payments:Stripe:Publishable_Key"];
            return View();
        }

        // TODO: implement a POST Payment action method to handle the payment response

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

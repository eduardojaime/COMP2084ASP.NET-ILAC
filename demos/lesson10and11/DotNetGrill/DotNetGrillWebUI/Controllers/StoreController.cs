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
using Stripe.Checkout;
using Stripe;

namespace DotNetGrillWebUI.Controllers
{
    // This controller will handle shopping cart experience
    public class StoreController : Controller
    {
        // Dependency injection
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration; // create private field to hold instance
        // modify constructor to require an IConfiguration instance
        public StoreController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration; // assign instance to private field
        }

        // GET: Store
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // Delete all other methods, they are not needed for this section
        // GET: Browse/5
        public async Task<IActionResult> Browse(int id)
        {
            // retrieve a list of products by category
            // LINQ query
            var products = _context.Products        // SELECT * FROM Products
                .Where(p => p.CategoryId == id)     // WHERE CategoryId = id
                .OrderBy(p => p.Name)               // ORDER BY Name
                .ToList();

            // retrieve the category name
            ViewBag.CategoryName = _context.Categories.Find(id).Name;

            // return the view with the list of products
            return View(products);
        }

        // POST: /AddToCart
        // Use [FromForm] to bind the parameters to the form data (input fields)
        public IActionResult AddToCart([FromForm] int ProductId, [FromForm] int Quantity)
        {
            // Identify user buying the product
            var customerId = GetCustomerId();
            // Retrieve price from products table
            var price = _context.Products.Find(ProductId).Price;
            // Create cart object and save
            var cart = new Cart()
            {
                ProductId = ProductId,
                Quantity = Quantity,
                Price = price,
                DateCreated = DateTime.UtcNow,
                CustomerId = customerId
            };
            _context.Carts.Add(cart);
            _context.SaveChanges();
            // Redirect to cart view
            return Redirect("Cart");
        }

        // GET: /Cart
        public IActionResult Cart()
        {
            // retrieve customerId
            var customerId = GetCustomerId();
            // query the cart table for all items with the customerId
            var carts = _context.Carts                      // SELECT * FROM Carts
                .Include(c => c.Product)                    // JOIN Products ON Carts.ProductId = Products.ProductId
                .Where(c => c.CustomerId == customerId)     // WHERE CustomerId = customerId
                .OrderByDescending(c => c.DateCreated)      // ORDER BY DateCreated DESC
                .ToList();
            // Calculate total price and render as string in currency format
            ViewBag.TotalAmount = carts.Sum(c => (c.Price * c.Quantity));
            // return view with the list of items
            return View(carts);
        }

        // GET: /RemoveFromCart/5
        public IActionResult RemoveFromCart(int id)
        {
            // Retrieve item from cart
            var cart = _context.Carts.Find(id);
            // Remove from carts collection
            _context.Carts.Remove(cart);
            // Save changes
            _context.SaveChanges();
            // Redirect
            return RedirectToAction("Cart");
        }

        // GET /Checkout
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        // POST /Checkout (Form data)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        // Model binder initializes an object of type Order and binds the form data to it
        public IActionResult Checkout(
            [Bind("FirstName,LastName,Address,City,Province,PostalCode")] Order order)
        {
            // populate 3 special fields: customerid, datecreated, total
            order.CustomerId = GetCustomerId();
            order.DateCreated = DateTime.UtcNow; // always use UTC time in DBs
            // calculate total by retrieving all cart items and summing the price * quantity for each
            order.Total = _context.Carts.Where(c => c.CustomerId == order.CustomerId)
                                        .Sum(c => (c.Price * c.Quantity));
            // store order in session to hold temporarily until payment is made
            HttpContext.Session.SetObject("Order", order);
            // redirect to payment page
            return RedirectToAction("Payment");
        }

        // GET /Payment
        public IActionResult Payment()
        {
            // retrieve order from session
            var order = HttpContext.Session.GetObject<Order>("Order");
            ViewBag.TotalAmount = order.Total;
            // Get configuration object via DI
            ViewBag.PublishableKey = _configuration["Payments:Stripe:PublishableKey"];

            return View();
        }

        // POST handler for /Store/Payment
        [HttpPost]
        public IActionResult Payment(string stripeToken)
        {
            // get order from session variable
            var order = HttpContext.Session.GetObject<DotNetGrillWebUI.Models.Order>("Order");
            // retrieve stripe config > Secret Key
            StripeConfiguration.ApiKey = _configuration["Payments:Stripe:SecretKey"];
            // create a stripe session object options
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long?)(order.Total * 100),
                        Currency = "cad",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "DotNetGrill Purchase"
                        },
                    },
                    Quantity = 1
                  },
                },
                PaymentMethodTypes = new List<string>
                {
                  "card"
                },
                Mode = "payment",
                SuccessUrl = "https://" + Request.Host + "/Store/SaveOrder",
                CancelUrl = "https://" + Request.Host + "/Store/Cart",
            };

            // create a stripe service object which will help us initialize the session
            var service = new SessionService();
            // initialize the session object
            Session session = service.Create(options);

            // pass and id back to the view (handle by javascript)
            return Json(new { id = session.Id });
        }

        // TODO: Implement SaveOrder method

        // TODO: Implement Order History

        // Helper Method
        private string GetCustomerId()
        {
            // Initialize variable
            string customerId = string.Empty;
            // Try to retrieve CustomerId from session
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

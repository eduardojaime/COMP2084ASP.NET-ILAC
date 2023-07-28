using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetGrill.Data;
using DotNetGrill.Models;
using Microsoft.AspNetCore.Authorization;
using DotNetGrill.Extensions;
using Stripe;
using Stripe.Checkout;

namespace DotNetGrill.Controllers
{
    public class StoreController : Controller
    {
        // Dependency Injection - dbcontext and configuration are services
        // registered in Program.cs or on Application Startup
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        // Controller receives an instance of dbcontext and configuration
        // from the app when it's created in memory
        public StoreController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Store
        public async Task<IActionResult> Index()
        {
            return _context.Categories != null ?
                        View(await _context.Categories.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }

        // GET: Store/Browse/1
        // Gets a categoryId value and shows a filtered list of products
        public async Task<IActionResult> Browse(int id)
        {
            // retrieve a list of products by category id
            // use dbcontext
            // linq query
            var products = await _context.Products
                .Where(p => p.CategoryId == id) // Similar to SQL WHERE P.CategoryId = @id
                .OrderBy(p => p.Name) // lambda expression, p means 'a record in the table' or 'a product'
                .ToListAsync();
            // send data to the view in two ways
            // option 1 - dynamic viewbag object
            // find the first category that matches the id and extract the name
            ViewBag.categoryName = _context.Categories.Find(id)?.Name;
            // option 2 - as a model for the view
            return View(products);
        }

        // GET: Store/AddToCart
        // data will come from the form elements: two input fields
        // model binder is a background process that links data sent from the request to parameters
        // in my action method
        public async Task<IActionResult> AddToCart([FromForm] int ProductId, [FromForm] int Quantity)
        {
            // get customer id
            var customerId = GetCustomerId();
            // query db to get product price
            var price = _context.Products.Find(ProductId).Price;
            // create and save cart object
            var cart = new Cart()
            {
                ProductId = ProductId,
                Quantity = Quantity,
                Price = price,
                DateCreate = DateTime.UtcNow, // Best Practice: always use UTC time
                CustomerId = customerId
            };
            // save
            _context.Carts.Add(cart);
            _context.SaveChanges();
            // redirect to cart view
            return Redirect("Cart");
        }

        // GET: Store/Cart
        public async Task<IActionResult> Cart()
        {
            string customerId = GetCustomerId();
            // return a list of all elements in the db associated to that customer id
            var carts = _context.Carts // SELECT * FROM CARTS c
                                       // How to include info from other tables ??
                .Include(c => c.Product) // JOIN Products p ON p.ProductId = c.ProductId
                .Where(c => c.CustomerId == customerId) // WHERE c.CustomerId = @
                .OrderByDescending(c => c.DateCreate) // ORDER BY c.DateCreate DESC
                .ToList();

            // Use viewbag object to pass data to the view
            // SELECT SUM(c.Price) FROM Carts c
            var total = carts.Sum(c => (c.Price * c.Quantity)).ToString("C");
            ViewBag.TotalAmount = total; // viewbag is a dynamic object

            return View(carts);
        }

        // GET: /Store/RemoveFromCart/{id}
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cart = _context.Carts.Find(id);
            _context.Carts.Remove(cart);
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }

        // GET: /Store/Checkout
        // Note: Only authenticated customers can complete a purchase
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View();
        }

        // POST: /Store/Checkout
        [Authorize]
        [ValidateAntiForgeryToken] // for security to prevent hijacking
        [HttpPost]
        // values in input fields are sent via form
        // use Model Binder to build this order object with info from form
        public async Task<IActionResult> Checkout(
            [Bind("FirstName,LastName,Address,City,Province,PostalCode")] Models.Order order
            )
        {
            // fill in missing info in object
            order.DateCreated = DateTime.UtcNow; // best practice
            order.CustomerId = GetCustomerId();
            var carts = _context.Carts
                        .Include(c => c.Product) // include every product connected to a cart, similar to JOIN in SQL
                        .Where(c => c.CustomerId == GetCustomerId())
                        .ToList();
            var total = carts.Sum(c => (c.Price * c.Quantity));
            order.Total = total;
            // store object in session, it will be saved to the DB once customer completes payment
            // write a custom extension method for storing objects
            HttpContext.Session.SetObject("Order", order);
            // redirect to payment page
            return RedirectToAction("Payment");
        }

        // GET: /Store/Payment
        public async Task<IActionResult> Payment()
        {
            // retrieve total amount and publishable key and send to the view
            var order = HttpContext.Session.GetObject<Models.Order>("Order");
            ViewBag.Total = order.Total;
            ViewBag.PublishableKey = _configuration["Payments:Stripe:PublishableKey"];
            return View();
        }

        // POST: /Store/Payment
        [HttpPost]
        public async Task<IActionResult> Payment(string stripeToken)
        {
            // retrieve order from session
            var order = HttpContext.Session.GetObject<Models.Order>("Order");
            // retrieve secret key from configuration
            StripeConfiguration.ApiKey = _configuration["Payments:Stripe:SecretKey"];
            // create a stripe session object with options and items
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions {
                            UnitAmount = (long)(order.Total * 100),
                            Currency = "cad",
                            ProductData = new SessionLineItemPriceDataProductDataOptions {
                                Name = "DotNetGrill Purchase"
                            },
                        },
                        Quantity = 1
                    },
                },
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = $"https://{Request.Host}/Store/SaveOrder",
                CancelUrl = $"https://{Request.Host}/Store/Cart",
            };
            // create a stripe service
            // initialize session
            var service = new SessionService();
            Session session = service.Create(options);
            // return ID to view for JS redirection
            return Json(new { id = session.Id });
        }

        // GET: /Store/SaveOrder
        public async Task<IActionResult> SaveOrder()
        {
            // retrieve order from session
            var order = HttpContext.Session.GetObject<Models.Order>("Order");
            // save order in db
            _context.Orders.Add(order);
            _context.SaveChanges();
            // get customer id
            var customerId = GetCustomerId();
            // retrieve cart information from db
            var carts = _context.Carts
                        .Where(c => c.CustomerId == GetCustomerId())
                        .ToList();
            // for each cart item we'll create a order item record
            foreach (var cartItem in carts)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price
                };
                _context.OrderItems.Add(orderItem);
            }
            _context.SaveChanges();
            // clear the cart
            foreach (var cartItem in carts)
            {
                _context.Carts.Remove(cartItem);
            }
            _context.SaveChanges();
            // redirect to order details page
            return RedirectToAction("Details", "Orders", new { @id = order.OrderId });
        }

        // This method uses the session object to store a value that idientifies users
        // Users can be anonymous or authenticated
        // Anonymous users will be identified by a GUID
        // Authenticated users will be identified by their email address
        private string GetCustomerId()
        {
            string customerId = string.Empty;
            // check the session object for a customer id
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
                // set session object with new value
                HttpContext.Session.SetString("CustomerId", customerId);
            }
            // return the value stored in the session object
            return HttpContext.Session.GetString("CustomerId");
        }
    }
}

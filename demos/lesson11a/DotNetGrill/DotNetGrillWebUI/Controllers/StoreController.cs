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
using DotNetGrillWebUI.Extensions; // to enable GetObject and SetObject methods from Session
using Stripe.Checkout;
using Stripe;

namespace DotNetGrillWebUI.Controllers
{
    public class StoreController : Controller
    {
        // Private field that will hold a reference to the dbcontext object provided via DI
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        // Constructor Method
        // ASP.NET MVC initializes the controller automatically when needed
        // and will provide a copy of all registered services via DI
        // if we need to use a specific service we can ask for it in the constructor
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
        public IActionResult AddToCart([FromForm] int ProductId, [FromForm] int Quantity)
        {
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
        public IActionResult Cart()
        {
            // retrieve customer id
            var customerId = GetCustomerId();
            // retrieve carts associated to them
            // LINQ Query
            var carts = _context.Carts                      // SELECT * FROM Carts
                .Include(c => c.Product)                    // JOIN Products ON Carts.ProductId = Products.ProductId
                .Where(c => c.CustomerId == customerId)     // WHERE CustomerId = @customerId
                .OrderByDescending(c => c.DateCreated)      // ORDER BY DateCreated DESC
                .ToList();
            // LINQ Methods (no need to implement foreach loops)
            var totalAmount = carts.Sum(c => (c.Price * c.Quantity)); // SELECT SUM(Price * Quantity) FROM Carts
            ViewBag.TotalAmount = totalAmount.ToString("C"); // format the total amount as currency
            // return a view with the list of carts
            return View(carts);
        }

        // GET: Store/RemoveFromCart/5 (5 is the cart id)
        public IActionResult RemoveFromCart(int? id)
        {
            // Retrieve the cart object by id
            var cart = _context.Carts.Find(id);
            // Remove it from the Carts DbSet
            _context.Carts.Remove(cart);
            // Save changes
            _context.SaveChanges();
            // Redirect to cart page
            return RedirectToAction("Cart");
        }

        // GET: Store/Checkout
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        // POST: Store/Checkout
        [HttpPost] // this action method will only respond to POST requests
        [ValidateAntiForgeryToken] // this attribute will check for a valid antiforgery token
        [Authorize] // this attribute will check if the user is authenticated
        // Model Binding: the parameters of the method will be automatically populated with the values
        // from the form, and inthis case create an object
        public IActionResult Checkout([FromForm] Order order)
        {
            // populate the 3 special order properties: DateCreated, CustomerId, Total
            order.DateCreated = DateTime.UtcNow; // always use UTC time to store datetimes in your dbs
            order.CustomerId = GetCustomerId();
            order.Total = _context.Carts
                            .Where(c => c.CustomerId == order.CustomerId)
                            .Sum(c => (c.Price * c.Quantity)); // calculate based on the cart items

            // Store order object in session store temporarily
            // Once payment is complete, I'll create the order record in the db
            HttpContext.Session.SetObject("Order", order);

            // Redirect to a payment page
            return RedirectToAction("Payment");
        }

        // GET: Store/Payment
        [Authorize]
        public IActionResult Payment()
        {
            // Retrieve the order object from the session store to get Total Amount
            var order = HttpContext.Session.GetObject<Order>("Order");
            ViewBag.TotalAmount = order.Total.ToString("C");
            ViewBag.TotalAmountInCents = order.Total * 100; // convert to cents for Stripe
            ViewBag.PublishableKey = _configuration["Payments:Stripe:Publishable_Key"];
            return View();
        }

        // POST: Store/Payment
        // This happens when user clicks on the "Pay" button in the Payment view
        [HttpPost]
        public IActionResult PaymentPost() { 
            // Retrieve the order object from the session store
            var order = HttpContext.Session.GetObject<Order>("Order");
            // Retrieve secret key from configuration
            StripeConfiguration.ApiKey = _configuration["Payments:Stripe:Secret_Key"];
            // Create a stripe charge object
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions // customize info for purchase
                    {
                      UnitAmount = (long)(order.Total * 100), // convert to cents
                      Currency = "cad",
                      ProductData = new SessionLineItemPriceDataProductDataOptions
                      {
                        Name = "DotNetGrill Purchase",
                      },
                    },
                    Quantity = 1,
                  },
                },
                PaymentMethodTypes = new List<string> // indicate which methods we accept
                {
                  "card",
                },
                Mode = "payment",
                SuccessUrl = $"https://{Request.Host}/Store/SaveOrder", // where to go if successful?
                CancelUrl = $"https://{Request.Host}/Store/Cart", // where to go if cancelled?
            };
            // Create a stripe service object to initialize the session
            var service = new SessionService();
            // Initialize Stripe session
            Session session = service.Create(options);
            // Pass session id back to the view for javascript to handle the payment
            return Json(new { id = session.Id });
        }

        // GET: Store/SaveOrder
        // This method will be called by Stripe after a successful payment
        public IActionResult SaveOrder() { 
            // Retrieve the order object from the session store
            var order = HttpContext.Session.GetObject<Order>("Order");
            // Save order in the db
            _context.Orders.Add(order);
            _context.SaveChanges();
            // Get CustomerId for storing cart items
            var customerId = GetCustomerId();
            // Retrieve cart items
            var carts = _context.Carts.Where(c => c.CustomerId == customerId);
            // Iterate over each cart item and store in db
            foreach (var cart in carts)
            {
                var orderDetail = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = cart.ProductId,
                    Quantity = cart.Quantity,
                    Price = cart.Price
                };
                _context.OrderItems.Add(orderDetail); // add order detail record to db
                _context.Carts.Remove(cart); // remove cart after using it
            }
            // Save changes 
            _context.SaveChanges();
            // Redirect to the order details page
            return RedirectToAction("Details", "Orders", new { @id = order.OrderId });
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

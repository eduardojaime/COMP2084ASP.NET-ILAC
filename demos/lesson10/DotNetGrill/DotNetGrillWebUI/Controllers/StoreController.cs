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
using Stripe;
using Stripe.Checkout;

namespace DotNetGrillWebUI.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Use DI to receive an instance of configuration object from the app
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
            // Alternative to  _context.Categories.Where(c=>c.CategoryId == id).FirstOrDefault().Name;
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
            // strongly typed syntax
            string customerId = GetCustomerId();
            // return a list of elements in the carts table by customerId
            var carts = _context.Carts                              // SELECT * FROM Carts c
                        .Include(c => c.Product)                    // JOIN Products p ON c.ProductId = p.ProductId
                        .Where(c => c.CustomerId == customerId)     // WHERE c.CustomerId = @customerId
                        .OrderByDescending(c => c.DateCreated)      // ORDER BY c.DateCreated DESC
                        .ToList();
            // Calculate Total and pass in ViewBag object
            // for each cart item calculate the result of multiplying price * quantity, and then calculate summatory
            var total = carts.Sum(c => (c.Price * c.Quantity));
            // ViewBag is a dynamic object, properties need to match exactly as we are naming them here in order to access their values in the View
            ViewBag.TotalAmount = total;

            return View(carts);
        }

        // GET handler for /Store/RemoveFromCart/ID
        public IActionResult RemoveFromCart(int id)
        {
            // retrieve the cart element
            var cartItem = _context.Carts.Find(id);
            // remove it from the carts list
            _context.Carts.Remove(cartItem);
            // save changes
            _context.SaveChanges();
            // redirect
            return RedirectToAction("Cart");
        }

        // GET handler for /Store/Checkout
        // Note: This will be a protected page, only users with an account can access it
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        // POST handler for /Store/Checkout
        // This is a protected action method, additionally use AntiForgeryToken to prevent POST requests from different sources than the form
        [HttpPost] // this action method responds to POST only
        [Authorize] // protected with login
        [ValidateAntiForgeryToken] // makes sure only our form can send requests to this method
        // Object binder Bind[()] receives values from form and uses them to create an instance of Order
        public IActionResult Checkout([Bind("FirstName,LastName,Address,City,Province,PostalCode")] Order order)
        {
            // Populate 3 special fields programmatically
            var customerId = GetCustomerId();
            order.DateCreated = DateTime.UtcNow; // always use UTC time
            order.CustomerId = customerId;

            var carts = _context.Carts
                        .Where(c => c.CustomerId == customerId)
                        .OrderByDescending(c => c.DateCreated)
                        .ToList();
            // Calculate Total and pass in ViewBag object
            // for each cart item calculate the result of multiplying price * quantity, and then calculate summatory
            var total = carts.Sum(c => (c.Price * c.Quantity));
            order.Total = total;

            // Store order in session, we will save to the DB once payment is completed
            HttpContext.Session.SetObject("Order", order);

            // Redirect to Payment page
            return RedirectToAction("Payment");
        }

        // GET handler for /Store/Payment
        public IActionResult Payment()
        {
            var order = HttpContext.Session.GetObject<Order>("Order");
            // Pass total in cents for stripe to handle
            ViewBag.Total = order.Total * 100;
            // Pass publishable key for javascript to work
            ViewBag.PublishableKey = _configuration["Payments:Stripe:PublishableKey"];
            return View();
        }

        // POST handler for /Store/Payment
        [HttpPost]
        public IActionResult Payment(string stripeToken)
        {
            // get order from session variable
            var order = HttpContext.Session.GetObject<Order>("Order");
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

        // GET handler for /Store/SaveOrder
        public IActionResult SaveOrder()
        {
            // retrieve order object
            var order = HttpContext.Session.GetObject<Order>("Order");
            // save it in the db
            _context.Orders.Add(order);
            _context.SaveChanges();
            // get customer id and cart items
            var customerId = GetCustomerId();
            var carts = _context.Carts.Where(c => c.CustomerId == customerId);
            // for loop to create corresponding orderitem records and clear carts
            foreach (var cartItem in carts) {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price,
                };
                _context.OrderItems.Add(orderItem);
                _context.Carts.Remove(cartItem);
            }
            _context.SaveChanges();

            //redirect to order details page
            return RedirectToAction("Details", "Order", new { @id = order.OrderId });
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

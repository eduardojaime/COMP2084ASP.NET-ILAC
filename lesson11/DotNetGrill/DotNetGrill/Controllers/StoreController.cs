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
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

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
                        .Include(c => c.Product) // include every product connected to a cart, similar to JOIN in SQL
                        .Where(c => c.CustomerId == customerId)
                        .OrderByDescending(c => c.DateCreated)
                        .ToList();

            // pass single value to view with the viewbag object
            // use LINQ methods to calculate sums and averages and other operations easily
            var total = carts.Sum(c => c.Price).ToString("C");
            ViewBag.TotalAmount = total; // Viewbag object is dynamic, after this line of code "TotalAmount" will be available to the app

            return View(carts);
        }

        // GET handler for /Store/RemoveFromCart
        public IActionResult RemoveFromCart(int id)
        {
            // Remove with LINQ
            // Retrieve element that you want to delete
            var cartItem = _context.Carts.Find(id);
            // Call .remove() from the dbset
            _context.Carts.Remove(cartItem);
            // save changes
            _context.SaveChanges();
            // redirect back to cart page
            return RedirectToAction("Cart");
        }

        // GET handler for /Store/Checkout
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        // POST handler for /Store/Checkout
        // POST is triggered by button inside a form
        [HttpPost]
        [ValidateAntiForgeryToken] // for security
        [Authorize]
        public IActionResult Checkout([Bind("FirstName,LastName,Address,City,Province,PostalCode")] DotNetGrill.Models.Order order)
        {
            // populate the 3 special order properties: Date, CustomerId, Total
            order.DateCreated = DateTime.UtcNow;
            order.CustomerId = GetCustomerId();
            // calculate total
            var carts = _context.Carts
                        .Include(c => c.Product) // include every product connected to a cart, similar to JOIN in SQL
                        .Where(c => c.CustomerId == GetCustomerId())
                        .OrderByDescending(c => c.DateCreated)
                        .ToList();
            var total = carts.Sum(c => c.Price);
            order.Total = total;

            // store in session object to hold this order temporarily until payment is made
            HttpContext.Session.SetObject("Order", order);

            // redirect to Payment page
            return RedirectToAction("Payment");
        }

        //GET handler for /Store/Payment
        public IActionResult Payment()
        {
            var order = HttpContext.Session.GetObject<DotNetGrill.Models.Order>("Order");

            // Pass total in cents
            ViewBag.Total = order.Total * 100;

            // pass publishable key to view to use in javascript code
            ViewBag.PublishableKey = _configuration["Stripe:PublishableKey"];

            return View();
        }
        // POST handler for /Store/Payment
        [HttpPost]
        public IActionResult Payment(string stripeToken)
        {
            // get order from session variable
            var order = HttpContext.Session.GetObject<DotNetGrill.Models.Order>("Order");
            // retrieve stripe config > Secret Key
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
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
            // get order from session
            var order = HttpContext.Session.GetObject<Models.Order>("Order");
            // save it in the db
            _context.Orders.Add(order);
            _context.SaveChanges();
            // get customer id
            var customerId = GetCustomerId();
            // get all cart items
            var cartItems = _context.Carts.Where(i => i.CustomerId == customerId);
            // save them in order details
            foreach (var item in cartItems)
            {
                var orderDetail = new Models.OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderItems.Add(orderDetail);
            }
            _context.SaveChanges();
            // clear the cart
            foreach (var item in cartItems)
            {
                _context.Carts.Remove(item);
            }
            _context.SaveChanges();
            // redirect to /Orders/Details
            return RedirectToAction("Details", "Orders", new { @id = order.OrderId });
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

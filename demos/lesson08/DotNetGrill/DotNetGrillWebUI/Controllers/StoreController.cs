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
    // This controller will handle shopping cart experience
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
        public IActionResult AddToCart([FromForm] int ProductId, [FromForm] int Quantity) {
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
        public IActionResult Cart() { 
            // retrieve customerId
            var customerId = GetCustomerId();
            // query the cart table for all items with the customerId
            var carts = _context.Carts
                .Where(c => c.CustomerId == customerId)
                .OrderByDescending(c => c.DateCreated)
                .ToList();
            // return view with the list of items
            return View(carts);
        }

        // Helper Method
        private string GetCustomerId() {
            // Initialize variable
            string customerId = string.Empty;
            // Try to retrieve CustomerId from session
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("CustomerId"))) {
                if (User.Identity.IsAuthenticated)
                {
                    customerId = User.Identity.Name;
                }
                else { 
                    customerId = Guid.NewGuid().ToString();
                }
                HttpContext.Session.SetString("CustomerId", customerId);
            }

            return HttpContext.Session.GetString("CustomerId");
        }
    }
}

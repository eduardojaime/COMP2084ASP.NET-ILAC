﻿using System;
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
        public async Task<IActionResult> Cart() {
            string customerId = GetCustomerId();
            // return a list of all elements in the db associated to that customer id
            var carts = _context.Carts
                .Where(c => c.CustomerId == customerId)
                .OrderByDescending(c => c.DateCreate)
                .ToList();
            return View(carts);
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
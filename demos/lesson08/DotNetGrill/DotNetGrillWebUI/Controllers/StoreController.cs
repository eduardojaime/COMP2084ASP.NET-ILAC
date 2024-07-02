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
    }
}

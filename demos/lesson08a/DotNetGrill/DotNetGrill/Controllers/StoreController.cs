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
            return View(await _context.Categories.ToListAsync());
        }

        // GET Store/Browse/ID
        public IActionResult Browse(int id) {
            // Filter products by id and return
            var products = _context.Products
                .Where(p => p.CategoryId == id) // filter by category id
                .OrderBy(p => p.Name) // sort by name
                .ToList(); // converts result to a list of products
            // two ways of sending data back to the view
            // 1) viewbag object
            ViewBag.CategoryName = _context.Categories.Find(id).Name; // finds a category by id and gets its name
            // 2) as a view model
            return View(products);
        }
    }
}

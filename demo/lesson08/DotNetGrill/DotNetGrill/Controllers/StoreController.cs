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
              return _context.Categories != null ? 
                          View(await _context.Categories.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }

        // GET: Store/Browse/1
        // Gets a categoryId value and shows a filtered list of products
        public async Task<IActionResult> Browser(int id) { 
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
    }
}

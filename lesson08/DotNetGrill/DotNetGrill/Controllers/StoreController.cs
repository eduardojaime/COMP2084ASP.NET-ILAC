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

        // GET handler for /Store/Category/{id}
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
    }
}

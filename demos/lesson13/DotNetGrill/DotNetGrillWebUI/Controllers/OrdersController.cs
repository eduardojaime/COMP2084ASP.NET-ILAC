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
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders > SHOW ALL ORDERS
        public async Task<IActionResult> Index()
        {
            // Rule: If user is admin they see all orders, if customer they see only their own orders
            if (User.IsInRole("Administrator"))
            {
                return View(await _context.Order.OrderByDescending(o => o.DateCreated).ToListAsync());
            }
            else
            {
                return View(await _context.Order
                                .Where(o => o.CustomerId == User.Identity.Name)
                                .OrderByDescending(o => o.DateCreated)
                                .ToListAsync());
            }

        }

        // GET: Orders/Details/5 > SHOW ORDER DETAILS FOR A SPECIFIC ORDER
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order                            // SELECT * FROM Order
                                        .Include(o => o.OrderItems)     // JOIN OrderItems ON Order.OrderId = OrderItems.OrderId
                                        .ThenInclude(o => o.Product)    // JOIN Product ON OrderItems.ProductId = Product.ProductId
                                        .FirstOrDefaultAsync(m => m.OrderId == id); // WHERE OrderId = id
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // REMOVE ALL OTHER ACTION AND HELPER METHODS AS WE DON'T NEED THEM FOR THIS SECTION
    }
}

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

namespace DotNetGrillWebUI
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            // Use case 1) Admin users will see ALL orders in the system
            if (User.IsInRole("Administrator"))
            {
                var orders = await _context.Orders
                    .OrderByDescending(o => o.DateCreated)
                    .ToListAsync();
                return View(orders);
            }
            // Use case 2) Regular customers will see only their orders
            else {
                var customerId = User.Identity.Name;
                var orders = await _context.Orders
                    .Where(o => o.CustomerId == customerId) // filter by email address
                    .OrderByDescending(o => o.DateCreated)
                    .ToListAsync();
                return View(orders);
            }            
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // retrieve orders + orderitems + products
            var order = await _context.Orders   // SELECT * FROM Orders
                .Include(o => o.OrderItems)     // JOIN OrderItems ...
                .ThenInclude(oi => oi.Product)  // JOIN Product ...
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

       // Removed all other methods, they're not needed
    }
}

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

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            // Admins see all orders
            if (User.IsInRole("Administrator"))
            {
                return View(await _context.Orders
                    .OrderByDescending(o => o.DateCreated)
                    .ToListAsync());
            }
            // Customers see only their own orders
            else
            {
                return View(await _context.Orders
                    .Where(o => o.CustomerId == User.Identity.Name) // email address
                    .OrderByDescending(o => o.DateCreated)
                    .ToListAsync());
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders // SELECT * FROM Orders o
                .Include(o => o.OrderItems) // JOIN OrderItems oi ON o.OrderId = oi.OrderId
                .ThenInclude(oi => oi.Product) // JOIN Products p ON oi.ProductId = p.ProductId
                .FirstOrDefaultAsync(m => m.OrderId == id); // WHERE o.OrderId = id
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}

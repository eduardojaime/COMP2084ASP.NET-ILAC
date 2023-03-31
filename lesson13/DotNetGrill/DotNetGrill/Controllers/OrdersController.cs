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
            if (User.IsInRole("Administrator"))
            {
                return View(await _context.Orders.OrderByDescending(o => o.DateCreated).ToListAsync());
            }
            else
            {
                return View(await _context.Orders
                    .Where(o => o.CustomerId == User.Identity.Name)
                    .OrderByDescending(o => o.DateCreated)
                    .ToListAsync());
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                            .Include(o => o.OrderItems) // tells linq to also include related orderitem records > JOIN
                            .ThenInclude(oi => oi.Product) // tells linq to also include related product records > JOIN
                            .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}

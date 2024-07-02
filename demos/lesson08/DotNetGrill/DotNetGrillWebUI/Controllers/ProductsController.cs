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

namespace DotNetGrillWebUI.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        [AllowAnonymous] // Allows access to the details page without logging in
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create > loads page
        public IActionResult Create()
        {
            // First param is the data to be displayed in the dropdown
            // Second param is the value to be saved in the database
            // Third param is the text to be displayed in the dropdown
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Products/Create > saves data
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Model Binder is used to bind the form data to the model
        // Photo is sent as a binary file, not as string, declare it as separate IFormFile parameter
        public async Task<IActionResult> Create([Bind("ProductId,Name,Description,Price,Rating,CategoryId")] Product product, IFormFile? photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null)
                {
                    // I'm uploading a photo
                    var fileName = UploadPhoto(photo); // new method, stores the photo in the wwwroot folder and returns the file name
                    product.Photo = fileName;
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,Price,Rating,CategoryId")] Product product,
                                                IFormFile? photo, string? currentPhoto)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null)
                    {
                        // I'm uploading a photo
                        var fileName = UploadPhoto(photo); // new method, stores the photo in the wwwroot folder and returns the file name
                        product.Photo = fileName;
                    }
                    else
                    {
                        // I'm not uploading a photo, retain same photo as before
                        product.Photo = currentPhoto;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        // Helper method to upload a photo and return its file name
        private static string UploadPhoto(IFormFile photo)
        {
            // Generate a unique file name using GUID
            // e.g. 123e4567-e89b-12d3-a456-426614174000_photo.jpg
            var fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            // set destination folder dinamically so it works both locally and on a server
            // I'll need to create the images/products folder in the wwwroot folder
            var uploadPath = System.IO.Directory.GetCurrentDirectory() 
                + "\\wwwroot\\images\\products\\"
                + fileName;
            // copy file to the destination folder
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                photo.CopyTo(fileStream);
            }
            // return the file name so it can be saved in the database
            return fileName;
        }
    }
}

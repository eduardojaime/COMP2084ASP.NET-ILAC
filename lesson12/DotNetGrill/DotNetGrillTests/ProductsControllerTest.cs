using DotNetGrill.Controllers;
using DotNetGrill.Data;
using DotNetGrill.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetGrillTests
{
    [TestClass]
    public class ProductsControllerTest
    {
        // When a test uses a database
        // we need to 'Mock' this data
        // use in-memory databases for testing
        private ApplicationDbContext _context;
        private ProductsController _controller;
        private List<Product> _products = new List<Product>();


        // Arrange step
        [TestInitialize]
        public void TestInitialize()
        {
            // instantiate in-memory db context
            // similar to registering your db in startup.cs
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                .Options;
            _context = new ApplicationDbContext(options);

            // mock some data
            // categories
            var category = new Category { CategoryId = 1, Name = "Breakfast" };
            _context.Categories.Add(category);
            _context.SaveChanges();

            // list of products
            var product1 = new Product { ProductId = 1, Name = "Tacos", Category = category };
            var product2 = new Product { ProductId = 2, Name = "Burritos", Category = category };
            var product3 = new Product { ProductId = 3, Name = "Tamales", Category = category };

            // add products to mock dbs
            _context.Products.Add(product1);
            _context.Products.Add(product2);
            _context.Products.Add(product3);
            _context.SaveChanges();

            // add products to local list
            _products.Add(product1);
            _products.Add(product2);
            _products.Add(product3);

            // instantiate the controller object with mock db context
            _controller = new ProductsController(_context);
        }

    }
}

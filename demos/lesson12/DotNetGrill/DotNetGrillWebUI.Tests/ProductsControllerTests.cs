using DotNetGrillWebUI.Controllers;
using DotNetGrillWebUI.Data;
using DotNetGrillWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetGrillWebUI.Tests
{
    // Make class public and sealed
    // Decorate with TestClass
    [TestClass]
    public sealed class ProductsControllerTests
    {
        private ApplicationDbContext _context;
        private List<Product> _products;
        private ProductsController _controller;

        // Declare test initializer method to configure data shared across tests
        [TestInitialize]
        public void TestInitialize()
        {
            // configure in memory db
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("DbName")
                .Options;
            _context = new ApplicationDbContext(options);

            // Mock data
            // Categories
            var cat1 = new Category
            {
                Name = "LUNCH",
            };
            _context.Categories.Add(cat1);

            // Products
            var prod1 = new Product
            {
                Category = cat1, // associate using navigation property
                Name = "TACOS",
                Description = "VERY DELICIOUS",
                Photo = null,
                Price = 10.0M,
                Rating = 10
            };
            _context.Products.Add(prod1);
            _context.SaveChanges();

            // keep a separate list of products to compare if they change in the db
            _products = new List<Product>() { prod1 };

            // instantiate a product controller object
            _controller = new ProductsController(_context);
        }
        // Test 1 > Index method returns a view
        [TestMethod]
        public void IndexReturnsAView()
        {
            // Arrange > handled in TestInitialize() above, no further prep needed

            // Act
            var result = _controller.Index();
            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ViewResult));
        }

        // Test 2 > Index method returns product data
        [TestMethod]
        public void IndexReturnsProductData() {
            // Arrange > handled above in TestInitialize()
            // Act
            var result = _controller.Index();
            var viewResult = result.Result as ViewResult;
            var model = viewResult.Model as List<Product>;
            
            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(_products.Count, model.Count);
            Assert.AreEqual(_products.First().Name, model.First().Name);
        }

    }
}

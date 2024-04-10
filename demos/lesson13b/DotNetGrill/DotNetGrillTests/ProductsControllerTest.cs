using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Add necessary references
using DotNetGrillWebUI.Controllers; // to access controller classes
using DotNetGrillWebUI.Data; // to access dbcontext class
using DotNetGrillWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // to access model classes

namespace DotNetGrillTests
{
    // Make class public and add TestClass attribute
    [TestClass]
    public class ProductsControllerTest
    {
        // Define global variables/objects
        private ProductsController _controller;
        private ApplicationDbContext _context;
        private List<Product> _products; // mock list of products to compare with dbcontext
        // We'll create multiple test methods to test some of the controller's methods
        // which means we'll reuse the controller, dbcontext and model objects
        // use a TestInitialize method to create these objects and setup data once
        // this method will run before each test method when executed
        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange - instantiate global objects, define global variables, etc.
            // Create a new instance of the dbcontext class
            // BEST PRACTICE: NEVER connect the unit test to a real db
            // Instead, use an in-memory db or mock data
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            // Populate the db context class with mock data
            // similar to what you do in the controller class
            var category = new Category { CategoryId = 1, Name = "Breakfast" };
            _context.Categories.Add(category);
            _context.SaveChanges();

            var product1 = new Product { ProductId = 1, Name = "Tacos", Description = "TEST", Price = 10, Rating = 10, Category = category };
            var product2 = new Product { ProductId = 2, Name = "Burritos", Description = "TEST", Price = 15, Rating = 9, Category = category };
            var product3 = new Product { ProductId = 3, Name = "Tamales", Description = "TEST", Price = 20, Rating = 8, Category = category };
            _context.Products.AddRange(product1, product2, product3);
            _context.SaveChanges();

            // Also add them to the mock list of products so we can compare with dbcontext
            _products = new List<Product> { product1, product2, product3 };

            // Create a new instance of the controller class
            _controller = new ProductsController(_context);
        }

        // Test 1) Verify that index returns a list of 3 products, same as mock list
        [TestMethod]
        public void IndexReturnsListOfProducts()
        {
            // Arrange - no need to do anything, it's already done in TestInitialize
            // Act - call the method to test
            var result = _controller.Index().Result as ViewResult;
            var model = result.Model as List<Product>;
            // To compare lists, we need to sort it's elements first
            var sortedModel = model.OrderBy(p => p.Name).ToList();
            var sortedProducts = _products.OrderBy(p => p.Name).ToList();
            // Assert - compare the result with the mock list of products
            // Use CollectionAssert to compare lists
            CollectionAssert.AreEqual(sortedProducts, sortedModel);
        }
        // Test 2) Verify that details returns NotFound when id doesn't exist in the db context
        [TestMethod]
        public void DetailsReturnsNotFoundForInvalidId()
        {
            // Arrange - database has been set, just define an invalid id
            int invalidId = 4; // id that doesn't exist in the db context
            // Act - call the method to test passing the invalid id
            var result = _controller.Details(invalidId).Result; // all methods are async tasks
            // Asset - verify result is of type NotFoundResult;
            Assert.IsInstanceOfType<NotFoundResult>(result);
        }
        // Test 3) Verify that details returns a product when id exists in the db context
        public void DetailsReturnsProductForValidId()
        {
            // Arrange - database has been set, just define a valid id
            int validId = 1; // id that exists in the db context
            // Act - call the method to test passing the valid id
            var result = _controller.Details(validId).Result as ViewResult;
            // Extract model
            
            // Assert - verify that the result is of type Product
        }
    }
}

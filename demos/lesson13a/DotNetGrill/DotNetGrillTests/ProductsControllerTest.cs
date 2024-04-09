using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Add all the necessary namespaces that you need before anything else
using DotNetGrillWebUI.Controllers; // access to the ProductsController class
using DotNetGrillWebUI.Models; // access to the Product class
using DotNetGrillWebUI.Data; // access to the ApplicationDbContext class
using Microsoft.EntityFrameworkCore; // access to the DbContextOptionsBuilder class
using Microsoft.AspNetCore.Mvc;

namespace DotNetGrillTests
{
    // Make sure to make the class public and add [TestClass] attribute
    [TestClass]
    public class ProductsControllerTest
    {
        // Objective: Test the CRUD operations of the ProductsController
        // Best Practice: Use Arrange, Act, Assert pattern
        // Prepare an in-memory database for testing. NEVER connect to a real database in unit tests.
        // Use a package like Microsoft.EntityFrameworkCore.InMemory to create an in-memory database.
        // Install if required.

        // Assert Step for ALL tests
        // Declare variables to hold reusable objects to use in all my test methods
        private ApplicationDbContext _context;
        private ProductsController _controller;
        private List<Product> _products; // mock list of products to use in tests

        // Use TestInitialize attribute to run code before each test
        [TestInitialize]
        public void TestInitialize()
        {
            // instantiate the DbContextOptionsBuilder class and initialize the in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Create a unique database name
                                .Options;
            _context = new ApplicationDbContext(options);
            // Mock some product data: Categories, then Products
            // Save data to context object
            var category = new Category { CategoryId = 1, Name = "Breakfast" };
            _context.Categories.Add(category);
            _context.SaveChanges(); // same as when using _context in the controller classes
            // Make sure to include ALL required attributes
            var product1 = new Product { ProductId = 1, Name = "Tacos", Description = "Test", Price = 10, Rating = 10, Category = category };
            var product2 = new Product { ProductId = 2, Name = "Burritos", Description = "Test", Price = 10, Rating = 10, Category = category };
            var product3 = new Product { ProductId = 3, Name = "Tamales", Description = "Test", Price = 10, Rating = 10, Category = category };
            _context.Products.AddRange(product1, product2, product3); // add 3 products to the context
            _context.SaveChanges();

            // Also add them to the mock list of products
            _products = new List<Product> { product1, product2, product3 };

            // Instantiate the ProductsController object
            _controller = new ProductsController(_context);
        }

        // Test 1) Test that Index method returns a list of products
        [TestMethod]
        public void IndexReturnsListOfProducts()
        {
            // Arrange: No need to arrange anything here, everything is already set up in TestInitialize() 
            // Act: Call the Index method of the controller
            var result = _controller.Index().Result as ViewResult;
            var model = result.Model as List<Product>;
            // sort the list of products by name to compare it later with the mock list
            var sortedModel = model.OrderBy(p => p.Name).ToList();
            // sort the mock list of products by name to compare it later with the model
            var sortedProducts = _products.OrderBy(p => p.Name).ToList();
            // Assert: Check that the result is not null and that the model is a list of products
            // Use CollectionAssert to compare the two lists
            CollectionAssert.AreEqual(sortedProducts, sortedModel);
        }

        // Test 2) Test that Details method returns NotFound when id is null or a non-existent product id
        [TestMethod]
        public void DetailsReturnsNotFoundWhenIdDoesntExist() { 
            // Arrange: data has already been prepared, set non-existent id
            int nonExistentId = 4;
            // Act: Call the Details method of the controller with a non-existent product id
            var result = _controller.Details(nonExistentId).Result as NotFoundResult;
            // Assert: Verify result returned statusCode 404 NOT FOUND
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}

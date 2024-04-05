using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Add references to the necessary namespaces
using DotNetGrillWebUI.Controllers; // to access ProductsController class
using DotNetGrillWebUI.Models; // to access Product and Category classes
using DotNetGrillWebUI.Data; // to access ApplicationDbContext class
using Microsoft.AspNetCore.Mvc; // to access result types
using Microsoft.EntityFrameworkCore; // to access UseInMemoryDatabase method

namespace DotNetGrillTests
{
    // Make class public and add the [TestClass] attribute
    [TestClass]
    public class ProductsControllerTests
    {
        // Define global variables
        private ProductsController _controller; // to test functionality
        private ApplicationDbContext _context; // to access mock database
        private List<Product> _products; // to compare data

        // We'll create multiple test methods to test the ProductsController class
        // Each test method will require an instance of ProductsController and ApplicationDbContext
        // Use the TestInitialize method prepare the data before each test method is executed
        [TestInitialize]
        public void TestInitialize()
        {
            // Global Arrange Step - it applies to ALL test methods in this class
            // Initialize global variables, such as _context and _controller
            // Mock data for the test methods
            // BEST PRACTICE: Never use real data in tests
            // Use something like in-memory database or mock data to test
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options; // guid generates a unique database name
            _context = new ApplicationDbContext(options);

            // At this point the db is empty, so we need to add test data
            var category = new Category { CategoryId = 1, Name = "Breakfast" };
            _context.Categories.Add(category); // same as in the controller
            _context.SaveChanges(); // save changes to the in-memory database

            var product1 = new Product { ProductId = 1, Name = "Tacos", Description = "TEST", Price = 10, Rating = 10, Category = category };
            var product2 = new Product { ProductId = 2, Name = "Burritos", Description = "TEST", Price = 10, Rating = 10, Category = category };
            var product3 = new Product { ProductId = 3, Name = "Tamales", Description = "TEST", Price = 10, Rating = 10, Category = category };
            _context.Products.AddRange(product1, product2, product3);
            _context.SaveChanges();

            // also store in mock list of products to compare with the data from the controller
            _products = new List<Product> { product1, product2, product3 };

            // Now that I have a context object with data, create a controller object
            _controller = new ProductsController(_context);
        }

        // 1) Verify that index returns a list of products
        [TestMethod]
        public void IndexReturnsAViewResultWithListOfProducts()
        { 
            // Arrange - nothing to do here, everything is done in TestInitialize
            // Act - call the Index method of the controller, and extract the model from the result
            // use .Result to get the actual result from the async method
            var result = _controller.Index().Result as ViewResult; // cast the result to a ViewResult
            var model = result.Model as List<Product>; // cast the model to a List<Product>
            // BEST PRACTICE: In order to compare lists we need to sort the data
            var sortedModel = model.OrderBy(p => p.Name).ToList();
            var sortedProducts = _products.OrderBy(p => p.Name).ToList();
            // Assert - compare the model with the mock data
            // CollectionAssert specializes in comparing collections
            CollectionAssert.AreEqual(sortedModel, sortedProducts);
        }

        // 2) Verify that details returns notfound when id is not in the database
        [TestMethod]
        public void DetailsReturnsNotFoundWhenIdIsInvalid()
        {
            // Arrange - not much to do here, only define an invalid id to test in this method
            int invalidId = 4;
            // Act - call the Details method of the controller with an invalid id
            var result = _controller.Details(invalidId).Result;
            // Assert - check if the result is of type NotFoundResult
            Assert.IsInstanceOfType<NotFoundResult>(result);
        }

    }
}

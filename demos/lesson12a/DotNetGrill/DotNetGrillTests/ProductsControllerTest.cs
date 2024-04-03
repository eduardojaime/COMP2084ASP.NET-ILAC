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

            var product1 = new Product { ProductId = 1, Name = "Tacos", Category = category };
            var product2 = new Product { ProductId = 2, Name = "Burritos", Category = category };
            var product3 = new Product { ProductId = 3, Name = "Tamales", Category = category };
            _context.Products.AddRange(product1, product2, product3); // add 3 products to the context
            _context.SaveChanges(); 

            // Also add them to the mock list of products
            _products = new List<Product> { product1, product2, product3 };

            // Instantiate the ProductsController object
            _controller = new ProductsController(_context);
        }

        // Test 1) Test that Index method returns a list of products
        

        // Test 2) Test that Details method returns NotFound when id is null or a non-existent product id


    }
}

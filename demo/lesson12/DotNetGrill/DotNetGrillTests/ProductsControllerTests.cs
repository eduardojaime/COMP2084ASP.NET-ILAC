using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetGrillTests
{
    [TestClass]
    public class ProductsControllerTests
    {
        // Set class level variables
        private ApplicationDbContext _context; // null object, needs instantiation
        private ProductsController _controller;
        private List<Product> _products; // keep this list to compare with the DB
        // Arrange > Initialize objects, dbs, mock data, etc...
        [TestInitialize] // runs automatically before each test
        public void Setup()
        {
            // initialize in-memory db
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options); // db is empty at this point
            // mock some data
            var category = new Category { CategoryId = 1, Name = "Dinner Menu" };
            var product1 = new Product { ProductId = 1, Category = category, Name = "Tacos", Price = 10 };
            var product2 = new Product { ProductId = 2, Category = category, Name = "Quesadilla", Price = 10 };
            var product3 = new Product { ProductId = 3, Category = category, Name = "Burrito", Price = 10 };
            // add mock data to db (similar to adding via EF in a Controller in the main project)
            _context.Categories.Add(category);
            _context.SaveChanges();
            _context.Products.Add(product1);
            _context.Products.Add(product2);
            _context.Products.Add(product3);
            _context.SaveChanges(); // database contains 1 cat and 3 products now!
            // add mock products to list<product>
            _products = new List<Product> { product1, product2, product3 }; // now local list contains 3 products
            // instantiate controller object
            _controller = new ProductsController(_context); // pass in-memory dbcontext object 
        }
        // Test 1 > making sure that index returns something (view result)
        // Test 2 > making sure that index loads some data
        // Test 3 > making sure I get a not found result if I try to view details of invalid ID
        // Test 4 > making sure I can add a new product to the DB
    }
}

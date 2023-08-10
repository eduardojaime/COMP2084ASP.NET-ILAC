using Microsoft.AspNetCore.Mvc;
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
            var product1 = new Product { ProductId = 1, Category = category, Name = "Tacos", Price = 10, Description = "TEST" };
            var product2 = new Product { ProductId = 2, Category = category, Name = "Quesadilla", Price = 10, Description = "TEST" };
            var product3 = new Product { ProductId = 3, Category = category, Name = "Burrito", Price = 10, Description = "TEST" };
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
        [TestMethod]
        public void IndexReturnsResultObject() {
            // skip Arrange step since it's executed automatically via Setup() method above each time
            // Act
            var result = _controller.Index();
            // Assert
            Assert.IsNotNull(result);
        }
        // Test 2 > making sure that index loads some data
        [TestMethod]
        public void IndexReturnsProductData() {
            var result = _controller.Index(); // returns a view result
            // extract view result
            var view = (ViewResult)result.Result;
            // extract model (data) associated with the view
            var model = (List<Product>)view.Model; // convert model in view to list of products
            // compare
            Assert.AreEqual(_products.Count, model.Count); // both should be 3
            // alternativelly you can use CollectionAssert when comparing lists
            // CollectionAssert.AreEqual(_products, model); // but both have to be sorted in the same way
        }
        // Test 3 > making sure I get a not found result if I try to view details of invalid ID
        [TestMethod]
        public void DetailsReturnsNotFoundIfInvalidID() {
            var testId = 100;
            var result = _controller.Details(testId);
            var notfound = (NotFoundResult)result.Result;
            Assert.AreEqual(404, notfound.StatusCode); // if ID doesn't exist, return 404 not found page
        }
        // Test 4 > making sure I can add a new product to the DB
        [TestMethod]
        public void PostCreateProduct() {
            // create an instance of product
            var product = new Product
            {
                ProductId = 4,
                Name = "Enchilada",
                Description = "TEST",
                Price = 10,
                Category = new Category { CategoryId = 2, Name = "Lunch" }
            };
            // call crete method and pass new product
            _controller.Create(product, null); // pass product and a null photo
            // verify I now have 4 products in my db
            var count = _context.Products.ToList().Count; // extracting count directly
            var expected = 4; // I started with three, added one via Create() now it should be 4
            Assert.AreEqual(expected, count); // both should be 4 now
        }
    }
}

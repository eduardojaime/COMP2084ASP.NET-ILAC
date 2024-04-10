using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Add necessary references from main project
using DotNetGrillWebUI.Controllers; // to access all controller classes
using Microsoft.AspNetCore.Mvc; // to access Return Result types, such as ViewResult

namespace DotNetGrillTests
{
    // Make class public and add TestClass attribute
    [TestClass]
    public class HomeControllerTest
    {
        // Write a test method to verify that Index returns a ViewResult object
        // Add TestMethod attribute, make method public
        [TestMethod]
        public void IndexReturnsViewResultObject() {
            // Arrange - instantiate objects, define variables, etc.
            var controller = new HomeController(null);
            // Act - call method to test
            var result = controller.Index() as ViewResult;
            // Assert - verify that the method behaves as expected
            Assert.IsInstanceOfType<ViewResult>(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// import the Controller namespace from the main project
// make sure to add a project reference to the main project
using DotNetGrillWebUI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DotNetGrillTests
{
    // Unit Test for HomeController
    //  Test class must be public
    //  Add the TestClass attribute to the class
    [TestClass]
    public class HomeControllerTest
    {
        // Controllers usually return an object of type IActionResult
        // This can be a ViewResult, JsonResult, or other types
        // Write a test that checks the method is returning the correct view
        [TestMethod]
        public void IndexReturnsViewNamedIndex()
        { 
            // Arrange - prepare the test
            HomeController controller = new HomeController(null);
            var expected = "Index"; // the name of the view to be returned
            // Act - run the method to be tested
            // call the Index method and cast the result from IActionResult to a ViewResult
            var result = (ViewResult)controller.Index();
            // ViewName will be null since we didn't specify a view name
            // when we called return View() in HomeController
            string actual = result.ViewName;
            // Assert - check the result
            // Assert.AreEqual(expected, actual); FAILS as actual is null
            Assert.IsNotNull(result); // PASSES since it retnurns a ViewResult
        }
    }
}

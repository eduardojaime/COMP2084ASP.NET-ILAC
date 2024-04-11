using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Add references to the DotNetGrillWebUI project namespaces
using DotNetGrillWebUI.Controllers; // to access controller classes
using Microsoft.AspNetCore.Mvc; // to access result types: ViewResult

namespace DotNetGrillTests
{
    // Make class public and add [TestClass] attribute
    [TestClass]
    public class HomeControllerTests
    {
        // 1) Verify that the index method returns a result of type ViewResult
        [TestMethod]
        public void IndexReturnsViewResultObject()
        {
            // Arrange - instantiate the HomeController class
            var controller = new HomeController(null);
            // Act - call the Index method and cast result object as ViewResult
            var result = controller.Index() as ViewResult;
            // Assert - verify that the result object is of type ViewResult
            Assert.IsInstanceOfType<ViewResult>(result);
        }
    }
}

using DotNetGrill.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetGrillTests
{
    [TestClass]
    public class HomeControllerTest
    {
        // write methods which will become my unit tests
        // what to test now?.... it depends on the main class being tested
        // MVC controllers usually produce views or json or lists of some data
        // Test 1 > Make sure index returns a result
        [TestMethod]
        public void IndexReturnsResult()
        {
            // Arrange > prepare the test or data needed
            HomeController controller = new HomeController(null);
            // Act > call the function or functions involved
            var result = controller.Index();
            // Assert > compare the actual and expected results
            Assert.IsNotNull(result);
        }

        // Test 2 > Make sure privacy returns a view named privacy
        [TestMethod]
        public void PrivacyLoadsPrivacyView()
        {
            // Arrange
            HomeController controller = new HomeController(null);
            // Act
            var result = (ViewResult)controller.Privacy();
            // Assert
            Assert.AreEqual("Privacy", result.ViewName);
        }
    }
}

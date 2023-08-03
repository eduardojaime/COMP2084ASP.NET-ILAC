using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// DotNetGrill.Controllers imported in global usings.cs

namespace DotNetGrillTests
{
    [TestClass]
    public class HomeControllerTests
    {
        // Test calling Index() returns something (not null object)
        [TestMethod]
        public void IndexReturnsObject() {
            // Arrange
            var controller = new HomeController(null);
            // Act
            var result = controller.Index();
            // Assert
            Assert.IsNotNull(result);
        }
    }
}

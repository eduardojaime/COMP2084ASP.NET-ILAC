// local usings and imports
namespace DotNetGrillTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test2plus2equals4() {
            // Arrange
            int x = 2, y = 2, expected = 4;
            // Act
            int result = x + y;
            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
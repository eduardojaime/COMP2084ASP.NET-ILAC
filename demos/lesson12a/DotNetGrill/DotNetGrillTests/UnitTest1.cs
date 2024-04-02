namespace DotNetGrillTests
{
    // TestClass attribute is used to define a class that contains unit tests.
    [TestClass]
    public class UnitTest1
    {
        // You will have 1 or more test methods in a test class.
        // TestMethod attribute is used to define a method that contains a unit test.
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            int x = 10;
            int y = 5; // to make it fail
            int expected = 15;
            // Act
            var actual = x + y;
            // Assert
            Assert.AreEqual(expected, actual); // is (x+y) == 15 ??
        }
    }
}
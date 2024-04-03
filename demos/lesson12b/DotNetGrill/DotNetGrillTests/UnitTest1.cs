namespace DotNetGrillTests
{
    // TestClass attribute is used to define a class that contains unit tests.
    // Class must be public and contain at least one public method that has the TestMethod attribute.
    [TestClass]
    public class UnitTest1
    {
        // TestMethod attribute is used to define a method that is a unit test.
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange - define variables and set up the test data
            int x = 10;
            int y = 9;
            int expected = 15;
            // Act - perform the operation that you want to test
            int actual = x + y; // this should be a call to the method that you want to test
            // Assert - verify that the result is what you expected
            Assert.AreEqual(expected, actual); // should pass
        }
    }
}
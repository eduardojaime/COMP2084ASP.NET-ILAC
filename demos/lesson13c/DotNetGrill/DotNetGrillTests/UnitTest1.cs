namespace DotNetGrillTests
{
    // MSTest
    // TestClass attribute is used to define a class that contains unit tests.
    // Class must be public and contain a public method that has the TestMethod attribute.
    [TestClass]
    public class UnitTest1
    {
        // You will have 1 or more test methods in a test class.
        // Use TestMethod attribute to define a method that contains a unit test.
        [TestMethod]
        public void TestMethod1()
        {
            // 1) Test that 10 + 5 = 15
            // Arrange - Initizalize objects/variables and set values
            int x = 10;
            int y = 5;
            int expected = 15;
            // Act - Perform the operation to be tested
            var calculated = x + y; // in a real unit test this will be a method call
            // Assert - Verify the result
            Assert.AreEqual(expected, calculated);
        }
    }
    // TestClass and TestMethods are attributes used by Visual Studio to identify tests.
}
namespace DotNetGrillWebUI.Tests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            int x = 2;
            int y = 8;
            int expected = 10;

            // Act
            int actual = x + y;

            // Assert
            Assert.AreEqual(expected, actual); // this should be true
        }
    }
}

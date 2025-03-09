using WebhookTester.Core.Services;

namespace WebhookTester.Core.Tests
{
    [TestClass]
    public sealed class TokenServiceTests
    {
        [TestMethod]
        public void CreateToken_ShouldReturnNewGuid()
        {
            // Arrange
            var tokenService = new TokenService();

            // Act
            var result = tokenService.CreateToken();
            var result2 = tokenService.CreateToken();

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result2.Success);
            Assert.AreNotEqual(Guid.Empty, result.Data);
            Assert.AreNotEqual(result.Data, result2.Data);
        }
    }
}

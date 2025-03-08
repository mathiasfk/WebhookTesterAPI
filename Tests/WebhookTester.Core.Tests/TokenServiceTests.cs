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
            var token = tokenService.CreateToken();
            var token2 = tokenService.CreateToken();

            // Assert
            Assert.AreNotEqual(Guid.Empty, token);
            Assert.AreNotEqual(token, token2);
        }
    }
}
